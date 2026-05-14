using System.ComponentModel.DataAnnotations;

namespace Pulperia.Models;

public class Venta
{
    [Key]
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public decimal Total { get; set; }

    // Mejor como enum (no decimal)
    public string MetodoPago { get; set; }

    public decimal MontoRecibido { get; set; }

    public decimal Cambio { get; set; }

    public string? Notas { get; set; }

    public string? ClienteId { get; set; }

    public bool Pagado { get; set; }

    // Relación DetalleVenta
    public ICollection<DetalleVenta> DetalleVentas { get; set; }
        = new List<DetalleVenta>();
}