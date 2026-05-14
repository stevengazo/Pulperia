using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;

public class Producto
{
    [Key]
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Presentacion { get; set; } = null!;

    public decimal PrecioCosto { get; set; }

    public decimal PrecioVenta { get; set; }

    public int StockActual { get; set; }

    public int StockMinimo { get; set; }

    // FK Categoria
    public int? CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    // Relación ventas
    public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
}