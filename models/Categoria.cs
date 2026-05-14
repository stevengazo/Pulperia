

namespace Pulperia.Models;


public class Categoria
{
    public int Id {get;set; }
    public string Nombre { get;set;}

// Relation With Producto
    public ICollection<Producto> Productos {get;set;   }
}