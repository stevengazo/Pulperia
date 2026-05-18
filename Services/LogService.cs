using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pulperia.Data;
using Pulperia.Models;

namespace Pulperia.Services;

public class LogService
{
    private readonly IDbContextFactory<PulperiaDbContext> _factory;

    public LogService(IDbContextFactory<PulperiaDbContext> factory)
    {
        _factory = factory;
    }

    public async Task RegistrarAsync(
        string tabla,
        string accion,
        string registroId,
        string usuario,
        object? anterior = null,
        object? nuevo = null)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var log = new LogCambio
        {
            Tabla = tabla,
            Accion = accion,
            RegistroId = registroId,
            Usuario = usuario,
            ValoresAnteriores = anterior != null
                ? JsonSerializer.Serialize(anterior)
                : null,

            ValoresNuevos = nuevo != null
                ? JsonSerializer.Serialize(nuevo)
                : null,

            Fecha = DateTime.Now
        };

        db.LogsCambios.Add(log);

        await db.SaveChangesAsync();
    }
}