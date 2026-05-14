

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;


public class DetalleVenta
{
    [Key]
    public int Id { get; set; }

    // Relation with Venta
    [ForeignKey(nameof(Venta))]
    public int VentaId { get; set; }
    public Venta? Venta { get; set; }

    // Relation With Producto
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioHistorico { get; set; }
    public decimal CostoUnitarioHistorico { get; set; }
}