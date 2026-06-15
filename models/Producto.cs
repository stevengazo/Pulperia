using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;

public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La presentación es obligatoria.")]
    [StringLength(50, ErrorMessage = "La presentación no puede superar los 50 caracteres.")]
    public string Presentacion { get; set; } = null!;

    [Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
    public decimal PrecioCosto { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
    public int StockActual { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    // FK Categoria
    [Required(ErrorMessage = "Seleccione una categoría.")]
    public int? CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public Categoria? Categoria { get; set; }

    // Relación ventas
    public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    public ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

}