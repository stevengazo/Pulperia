using Pulperia.Models;

namespace Pulperia.Services;

public class EmpleadoService
{
    private readonly Supabase.Client _supabase;

    public EmpleadoService(Supabase.Client supabase) => _supabase = supabase;

    public async Task<List<Empleado>> GetAllAsync()
    {
        var result = await _supabase
            .From<Empleado>()
            .Where(e => e.Activo == true)
            .Get();

        return result.Models.ToList();
    }

    public async Task<Empleado?> GetByIdAsync(string id)
    {
        var result = await _supabase
            .From<Empleado>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Single();

        return result;
    }
}