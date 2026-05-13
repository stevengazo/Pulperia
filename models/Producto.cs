


namespace Pulperia.Models;


public class Producto
{
    public int Id { get; set; }
    public string CodigoBarras { get; set; } = string.Empty; // SKU
    public string Nombre { get; set; } = string.Empty;
    public string Presentacion { get; set; } = string.Empty; // Ej: "1.8kg" o "Botella 600ml"
    
    public decimal PrecioCosto { get; set; }  // Para calcular ganancias
    public decimal PrecioVenta { get; set; }  // Lo que paga el cliente
    
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }      // Para alertas de "Stock Bajo"
    
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    
}