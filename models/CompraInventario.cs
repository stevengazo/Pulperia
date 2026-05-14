using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;

public class CompraInventario
{
    [Key]
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    // FK Proveedor
    public int? ProveedorId { get; set; }

    [ForeignKey(nameof(ProveedorId))]
    public Proveedor? Proveedor { get; set; }

    public string? Notas { get; set; }

    // Relación detalles
    public ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    // Total calculado (RECOMENDADO)
    [NotMapped]
    public decimal TotalCalculado => DetalleCompras?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
}