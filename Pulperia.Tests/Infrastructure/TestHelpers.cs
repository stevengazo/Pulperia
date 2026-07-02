using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Pulperia.Data;
using Pulperia.Models;
using Pulperia.Services;

namespace Pulperia.Tests.Infrastructure;

/// <summary>
/// Utilidades compartidas por las pruebas: construcción de servicios reales con
/// dependencias mínimas (sin Supabase ni red) y sembrado de datos de prueba.
/// </summary>
internal static class TestHelpers
{
    /// <summary>
    /// Crea un <see cref="LogService"/> real apuntando a la base de prueba.
    ///
    /// <see cref="LogService"/> depende de <see cref="AppSessionService"/> solo
    /// para leer el usuario actual. En pruebas construimos un
    /// <see cref="AppSessionService"/> con dependencias nulas: su constructor solo
    /// las almacena y nunca las usa mientras <c>CurrentUser</c> siga en null
    /// (que es el valor por defecto). Así evitamos levantar Supabase.
    /// </summary>
    public static LogService CreateLogService(IDbContextFactory<PulperiaDbContext> factory)
    {
        var session = new AppSessionService(
            supabase: null!,
            serviceProvider: null!,
            logger: NullLogger<AppSessionService>.Instance);

        return new LogService(factory, session, NullLogger<LogService>.Instance);
    }

    /// <summary>Crea un <see cref="VentaService"/> real sobre la base de prueba.</summary>
    public static VentaService CreateVentaService(IDbContextFactory<PulperiaDbContext> factory)
    {
        var log = CreateLogService(factory);
        return new VentaService(factory, log, NullLogger<VentaService>.Instance);
    }

    /// <summary>
    /// Inserta un producto de prueba y devuelve su Id. La categoría se deja nula
    /// (es opcional en el modelo).
    /// </summary>
    public static int SeedProducto(
        IDbContextFactory<PulperiaDbContext> factory,
        string nombre = "Producto de prueba",
        decimal precioVenta = 500m,
        decimal precioCosto = 300m,
        int stock = 10,
        int stockMinimo = 1)
    {
        using var db = factory.CreateDbContext();

        var producto = new Producto
        {
            Nombre = nombre,
            Presentacion = "unidad",
            PrecioCosto = precioCosto,
            PrecioVenta = precioVenta,
            StockActual = stock,
            StockMinimo = stockMinimo
        };

        db.Producto.Add(producto);
        db.SaveChanges();

        return producto.Id;
    }

    /// <summary>Construye una línea de carrito para un producto.</summary>
    public static DetalleVenta LineaCarrito(int productoId, int cantidad, decimal precio, decimal costo = 0m) =>
        new()
        {
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitarioHistorico = precio,
            CostoUnitarioHistorico = costo
        };

    /// <summary>Devuelve el stock actual de un producto leyéndolo de la base.</summary>
    public static int GetStock(IDbContextFactory<PulperiaDbContext> factory, int productoId)
    {
        using var db = factory.CreateDbContext();
        return db.Producto.AsNoTracking().Single(p => p.Id == productoId).StockActual;
    }
}
