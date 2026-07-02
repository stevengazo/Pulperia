using Microsoft.EntityFrameworkCore;
using Pulperia.Models;
using Pulperia.Services;
using Pulperia.Tests.Infrastructure;
using Xunit;

namespace Pulperia.Tests;

/// <summary>
/// Pruebas de <see cref="VentaService.ProcesarAsync"/>, el corazón del negocio.
///
/// Cubren:
///  - Validación de carrito vacío.
///  - Venta en efectivo: cálculo de total, cambio, estado pagado y descuento de
///    stock, más la creación del detalle y del registro de auditoría.
///  - Venta fiada: queda pendiente y sin cambio.
///  - Métodos no-efectivo: el monto recibido se iguala al total (sin cambio).
///  - Stock insuficiente: la operación se revierte por completo (no persiste la
///    venta ni altera el inventario).
/// </summary>
public class VentaServiceTests
{
    [Fact]
    public async Task ProcesarAsync_falla_si_el_carrito_esta_vacio()
    {
        using var factory = new SqliteTestFactory();
        var service = TestHelpers.CreateVentaService(factory);

        var resultado = await service.ProcesarAsync(
            carrito: new List<DetalleVenta>(),
            metodoPago: "Efectivo",
            montoRecibido: 0,
            empleadoId: null,
            autor: "test");

        Assert.False(resultado.Exito);
        Assert.Equal("El carrito está vacío.", resultado.Error);
    }

    [Fact]
    public async Task ProcesarAsync_venta_efectivo_descuenta_stock_y_calcula_cambio()
    {
        // Arrange: producto con 10 unidades a ₡500.
        using var factory = new SqliteTestFactory();
        var productoId = TestHelpers.SeedProducto(factory, stock: 10, precioVenta: 500m);
        var service = TestHelpers.CreateVentaService(factory);

        var carrito = new List<DetalleVenta>
        {
            TestHelpers.LineaCarrito(productoId, cantidad: 3, precio: 500m, costo: 300m)
        };

        // Act: paga con ₡2000 (total ₡1500 → cambio ₡500).
        var resultado = await service.ProcesarAsync(
            carrito, "Efectivo", montoRecibido: 2000m, empleadoId: null, autor: "cajero");

        // Assert
        Assert.True(resultado.Exito);
        Assert.NotNull(resultado.VentaId);

        using var db = factory.CreateDbContext();
        var venta = await db.Venta.AsNoTracking().SingleAsync();

        Assert.Equal(1500m, venta.Total);
        Assert.Equal(2000m, venta.MontoRecibido);
        Assert.Equal(500m, venta.Cambio);
        Assert.Equal("Efectivo", venta.MetodoPago);
        Assert.True(venta.Pagado);

        // Se creó el detalle...
        var detalle = await db.DetalleVenta.AsNoTracking().SingleAsync();
        Assert.Equal(productoId, detalle.ProductoId);
        Assert.Equal(3, detalle.Cantidad);

        // ...y el stock bajó de 10 a 7.
        Assert.Equal(7, TestHelpers.GetStock(factory, productoId));

        // ...y quedó auditado.
        var log = await db.LogsCambios.AsNoTracking()
            .SingleAsync(l => l.Tabla == "Venta" && l.Accion == "INSERT");
        Assert.Equal(resultado.VentaId!.Value.ToString(), log.RegistroId);
    }

    [Fact]
    public async Task ProcesarAsync_venta_fiada_queda_pendiente_y_sin_cambio()
    {
        using var factory = new SqliteTestFactory();
        var productoId = TestHelpers.SeedProducto(factory, stock: 5, precioVenta: 1000m);
        var service = TestHelpers.CreateVentaService(factory);

        var carrito = new List<DetalleVenta>
        {
            TestHelpers.LineaCarrito(productoId, cantidad: 2, precio: 1000m)
        };

        var resultado = await service.ProcesarAsync(
            carrito, "Fiado", montoRecibido: 0, empleadoId: "emp-1", autor: "cajero");

        Assert.True(resultado.Exito);

        using var db = factory.CreateDbContext();
        var venta = await db.Venta.AsNoTracking().SingleAsync();

        Assert.False(venta.Pagado);          // fiado = pendiente
        Assert.Equal(2000m, venta.Total);
        Assert.Equal(2000m, venta.MontoRecibido); // no-efectivo: monto = total
        Assert.Equal(0m, venta.Cambio);
        Assert.Equal("emp-1", venta.ClienteId);

        // El stock igual se descuenta aunque sea fiado.
        Assert.Equal(3, TestHelpers.GetStock(factory, productoId));
    }

    [Fact]
    public async Task ProcesarAsync_metodo_no_efectivo_no_genera_cambio()
    {
        using var factory = new SqliteTestFactory();
        var productoId = TestHelpers.SeedProducto(factory, stock: 4, precioVenta: 750m);
        var service = TestHelpers.CreateVentaService(factory);

        var carrito = new List<DetalleVenta>
        {
            TestHelpers.LineaCarrito(productoId, cantidad: 2, precio: 750m)
        };

        // Aunque se pase un monto recibido alto, al no ser efectivo se ignora.
        var resultado = await service.ProcesarAsync(
            carrito, "Tarjeta", montoRecibido: 99999m, empleadoId: null, autor: "cajero");

        Assert.True(resultado.Exito);

        using var db = factory.CreateDbContext();
        var venta = await db.Venta.AsNoTracking().SingleAsync();

        Assert.Equal(1500m, venta.MontoRecibido); // igual al total, no 99999
        Assert.Equal(0m, venta.Cambio);
        Assert.True(venta.Pagado);
    }

    [Fact]
    public async Task ProcesarAsync_con_stock_insuficiente_revierte_todo()
    {
        // Arrange: solo hay 2 unidades pero se intentan vender 5.
        using var factory = new SqliteTestFactory();
        var productoId = TestHelpers.SeedProducto(factory, stock: 2, precioVenta: 500m);
        var service = TestHelpers.CreateVentaService(factory);

        var carrito = new List<DetalleVenta>
        {
            TestHelpers.LineaCarrito(productoId, cantidad: 5, precio: 500m)
        };

        // Act
        var resultado = await service.ProcesarAsync(
            carrito, "Efectivo", montoRecibido: 5000m, empleadoId: null, autor: "cajero");

        // Assert: falló y avisó del stock.
        Assert.False(resultado.Exito);
        Assert.Contains("Stock insuficiente", resultado.Error);

        // La transacción se revirtió: no hay venta ni detalle...
        using var db = factory.CreateDbContext();
        Assert.Equal(0, await db.Venta.CountAsync());
        Assert.Equal(0, await db.DetalleVenta.CountAsync());

        // ...y el stock quedó intacto en 2.
        Assert.Equal(2, TestHelpers.GetStock(factory, productoId));
    }
}
