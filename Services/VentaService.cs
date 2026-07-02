using Microsoft.EntityFrameworkCore;
using Pulperia.Data;
using Pulperia.Models;

namespace Pulperia.Services;

/// <summary>
/// Resultado de procesar una venta. Éxito trae el Id generado; el fallo trae
/// un mensaje listo para mostrar al usuario.
/// </summary>
public record ResultadoVenta(bool Exito, int? VentaId, string? Error)
{
    public static ResultadoVenta Ok(int ventaId) => new(true, ventaId, null);
    public static ResultadoVenta Fallo(string error) => new(false, null, error);
}

/// <summary>
/// Encapsula el registro de una venta: creación de la cabecera, el detalle,
/// el descuento atómico de stock y la auditoría, todo dentro de una transacción.
/// Aísla la lógica de negocio de los componentes Blazor para poder probarla y
/// reutilizarla.
/// </summary>
public class VentaService
{
    private readonly IDbContextFactory<PulperiaDbContext> _factory;
    private readonly LogService _log;
    private readonly ILogger<VentaService> _logger;

    public VentaService(
        IDbContextFactory<PulperiaDbContext> factory,
        LogService log,
        ILogger<VentaService> logger)
    {
        _factory = factory;
        _log = log;
        _logger = logger;
    }

    /// <summary>
    /// Procesa una venta de forma transaccional. El descuento de stock usa una
    /// guarda (StockActual >= Cantidad) para evitar condiciones de carrera entre
    /// ventas simultáneas: si otra venta dejó el inventario insuficiente, se
    /// revierte todo.
    /// </summary>
    public async Task<ResultadoVenta> ProcesarAsync(
        IReadOnlyList<DetalleVenta> carrito,
        string metodoPago,
        decimal montoRecibido,
        string? empleadoId,
        string autor)
    {
        if (carrito is null || carrito.Count == 0)
            return ResultadoVenta.Fallo("El carrito está vacío.");

        var total = carrito.Sum(i => i.PrecioUnitarioHistorico * i.Cantidad);
        var esEfectivo = metodoPago == "Efectivo";
        var cambio = esEfectivo && montoRecibido > total ? montoRecibido - total : 0;

        await using var db = await _factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        try
        {
            var venta = new Venta
            {
                Fecha = DateTime.UtcNow,
                Total = total,
                MetodoPago = metodoPago,
                MontoRecibido = esEfectivo ? montoRecibido : total,
                Cambio = cambio,
                Notas = "",
                Autor = autor,
                Pagado = metodoPago != "Fiado",
                ClienteId = string.IsNullOrWhiteSpace(empleadoId) ? null : empleadoId
            };

            db.Venta.Add(venta);
            await db.SaveChangesAsync();

            foreach (var item in carrito)
            {
                // Insertamos un detalle nuevo (sin arrastrar la navegación Producto
                // del carrito, que pertenece a otro contexto) para evitar que EF
                // intente rastrear o insertar entidades ajenas a esta transacción.
                db.DetalleVenta.Add(new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitarioHistorico = item.PrecioUnitarioHistorico,
                    CostoUnitarioHistorico = item.CostoUnitarioHistorico
                });

                // Descuento atómico de stock con guarda: solo afecta la fila si
                // todavía hay existencias suficientes.
                var filas = await db.Producto
                    .Where(p => p.Id == item.ProductoId
                                && p.StockActual >= item.Cantidad)
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.StockActual,
                        p => p.StockActual - item.Cantidad));

                if (filas == 0)
                {
                    await tx.RollbackAsync();

                    var nombre = item.Producto?.Nombre ?? $"#{item.ProductoId}";
                    return ResultadoVenta.Fallo(
                        $"Stock insuficiente para {nombre}. " +
                        "Otra venta pudo haber modificado el inventario; revisa el carrito.");
                }
            }

            await db.SaveChangesAsync();
            await tx.CommitAsync();

            await _log.RegistrarAsync(
                tabla: "Venta",
                accion: "INSERT",
                registroId: venta.Id.ToString(),
                nuevo: new
                {
                    venta.Id,
                    venta.Total,
                    venta.MetodoPago,
                    venta.MontoRecibido,
                    venta.Cambio,
                    venta.Pagado,
                    venta.ClienteId,
                    Detalles = carrito.Select(d => new
                    {
                        d.ProductoId,
                        NombreProducto = d.Producto?.Nombre,
                        d.Cantidad,
                        d.PrecioUnitarioHistorico,
                        d.CostoUnitarioHistorico
                    })
                });

            return ResultadoVenta.Ok(venta.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al procesar la venta (Total {Total}, método {MetodoPago}).",
                total, metodoPago);

            return ResultadoVenta.Fallo("No se pudo procesar la venta. Intenta de nuevo.");
        }
    }
}
