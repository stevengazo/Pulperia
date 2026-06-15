using System.ComponentModel.DataAnnotations;

namespace Pulperia.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
    public string Nombre { get; set; } = null!;

    // Relation With Producto
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}