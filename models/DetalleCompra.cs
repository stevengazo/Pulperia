using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pulperia.Models;

public class DetalleCompra
{
    [Key]
    public int Id { get; set; }

    // FK correcta
    public int CompraId { get; set; }

    [ForeignKey(nameof(CompraId))]
    public CompraInventario CompraInventario { get; set; } = null!;

    // FK producto
    public int ProductoId { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public Producto Producto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    // Campo útil (recomendado)
    public decimal Total => Cantidad * PrecioUnitario;
}