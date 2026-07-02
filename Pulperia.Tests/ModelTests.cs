using Pulperia.Models;
using Xunit;

namespace Pulperia.Tests;

/// <summary>
/// Pruebas de las <b>propiedades calculadas</b> de los modelos de dominio.
///
/// Son cálculos simples pero críticos: alimentan totales de ventas, compras y
/// subtotales que el usuario ve y con los que se cobra. Un error aquí se traduce
/// en dinero mal calculado, por eso conviene fijar su comportamiento con pruebas.
/// </summary>
public class ModelTests
{
    // ── DetalleVenta.Subtotal = Cantidad * PrecioUnitarioHistorico ──────────

    [Theory]
    [InlineData(1, 500, 500)]
    [InlineData(3, 500, 1500)]
    [InlineData(0, 999, 0)]     // sin cantidad no hay subtotal
    [InlineData(2, 0, 0)]       // precio cero
    public void DetalleVenta_Subtotal_multiplica_cantidad_por_precio(
        int cantidad, decimal precio, decimal esperado)
    {
        var detalle = new DetalleVenta
        {
            Cantidad = cantidad,
            PrecioUnitarioHistorico = precio
        };

        Assert.Equal(esperado, detalle.Subtotal);
    }

    // ── DetalleCompra.Total = Cantidad * PrecioUnitario ─────────────────────

    [Theory]
    [InlineData(4, 250, 1000)]
    [InlineData(1, 1234.56, 1234.56)]
    public void DetalleCompra_Total_multiplica_cantidad_por_precio(
        int cantidad, decimal precio, decimal esperado)
    {
        var detalle = new DetalleCompra
        {
            Cantidad = cantidad,
            PrecioUnitario = precio
        };

        Assert.Equal(esperado, detalle.Total);
    }

    // ── CompraInventario.TotalCalculado = suma de sus detalles ──────────────

    [Fact]
    public void CompraInventario_TotalCalculado_suma_todos_los_detalles()
    {
        var compra = new CompraInventario
        {
            DetalleCompras = new List<DetalleCompra>
            {
                new() { Cantidad = 2, PrecioUnitario = 100 }, // 200
                new() { Cantidad = 3, PrecioUnitario = 50 },  // 150
            }
        };

        Assert.Equal(350m, compra.TotalCalculado);
    }

    [Fact]
    public void CompraInventario_TotalCalculado_es_cero_cuando_no_hay_detalles()
    {
        var compra = new CompraInventario
        {
            DetalleCompras = new List<DetalleCompra>()
        };

        Assert.Equal(0m, compra.TotalCalculado);
    }
}
