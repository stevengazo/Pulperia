using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;

public class DetalleVenta
{
    [Key]
    public int Id { get; set; }

    // FK Venta
    public int VentaId { get; set; }

    [ForeignKey(nameof(VentaId))]
    public Venta Venta { get; set; } = null!;

    // FK Producto
    public int ProductoId { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public Producto Producto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitarioHistorico { get; set; }

    public decimal CostoUnitarioHistorico { get; set; }

    // Campo calculado útil
    [NotMapped]
    public decimal Subtotal => Cantidad * PrecioUnitarioHistorico;
}