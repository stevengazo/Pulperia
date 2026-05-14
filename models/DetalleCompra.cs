


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;


public class DetalleCompra
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(CompraInventario))]
    public int? CompraId { get; set; }

    public CompraInventario? CompraInventario { get; set; }
    
    [ForeignKey(nameof(Producto))]
    public int? ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

}