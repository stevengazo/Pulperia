

using System.ComponentModel.DataAnnotations;

namespace Pulperia.Models;


public class Venta
{
    [Key]
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public decimal MetodoPago { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Cambio { get; set; }
    public string Notas { get; set; }
    public string ClienteId { get; set; }
    public bool Pagado { get; set; }

    // Relation With Detalle Venta
    public ICollection<DetalleVenta> DetalleVentas { get; set; }

}