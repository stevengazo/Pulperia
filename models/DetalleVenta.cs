

namespace Pulperia.Models;


public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }
    
    public int Cantidad { get; set; }
    public decimal PrecioUnitarioHistorico { get; set; } // Guardamos el precio al momento de la venta
    public decimal Subtotal => Cantidad * PrecioUnitarioHistorico;
}