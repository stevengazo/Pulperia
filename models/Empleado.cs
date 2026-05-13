using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Pulperia.Models;

[Table("Empleados")]
public class Empleado : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("codigo_empleado")]
    public string CodigoEmpleado { get; set; } = string.Empty;

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("rol")]
    public string Rol { get; set; } = string.Empty;

    [Column("activo")]
    public bool Activo { get; set; }

    [Column("photo")]
    public string? Photo { get; set; }

    [Column("departamento")]
    public int? Departamento { get; set; }

    [Column("empresa_id")]
    public Guid? EmpresaId { get; set; }
}