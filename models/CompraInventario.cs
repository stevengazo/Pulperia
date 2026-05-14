

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;


public class CompraInventario
{
    [Key]
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    /// Relation With Proveedor
    [ForeignKey(nameof(Proveedor))]
    public int? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public string? Notas { get; set; }

    public ICollection<DetalleCompra>? DetalleCompras {get;set;  }
}