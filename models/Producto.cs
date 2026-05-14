

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;


public class Producto
{
    [Key]
    public int Id { get; set; }
    //public string CodigoBarras {get;set;}
    public string Nombre { get; set; }
    public string Presentacion { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    // Relation With Categoria

    [ForeignKey(nameof(Categoria))]
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    
    // Relation with DetalleVenta
    public ICollection<DetalleVenta>? DetalleVentas {get;set;}
}