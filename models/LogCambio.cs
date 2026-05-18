namespace Pulperia.Models;

public class LogCambio
{
    public int Id { get; set; }

    public string Tabla { get; set; } = "";
    public string Accion { get; set; } = "";
    public string RegistroId { get; set; } = "";

    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }

    public string Usuario { get; set; } = "";

    public DateTime Fecha { get; set; } = DateTime.Now;

}