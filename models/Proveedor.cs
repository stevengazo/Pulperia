namespace Pulperia.Models;

public class Proveedor
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;

    // Relación con compras
    public ICollection<CompraInventario> CompraInventarios { get; set; }
        = new List<CompraInventario>();
}