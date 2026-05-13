

namespace Pulperia.Models; 


public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    
    // MetodoPago: "Efectivo", "SINPE", "Fiado"
    public string MetodoPago { get; set; } = "Efectivo"; 
    public decimal MontoRecibido { get; set; }
    public decimal Cambio { get; set; }
    public string? Notas { get; set; }

    // Relación con Cliente (opcional, obligatorio si es Fiado)
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public List<DetalleVenta> Detalles { get; set; } = new();
}