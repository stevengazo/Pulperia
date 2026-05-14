


namespace Pulperia.Models;


public class Proveedor
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    public ICollection<CompraInventario> CompraInventarios{get;set;}
}