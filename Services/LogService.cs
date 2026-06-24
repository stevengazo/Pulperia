using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pulperia.Data;
using Pulperia.Models;

namespace Pulperia.Services;

public class LogService
{
    private readonly IDbContextFactory<PulperiaDbContext> _factory;
    private readonly AppSessionService _session;
    private readonly ILogger<LogService> _logger;

    public LogService(
        IDbContextFactory<PulperiaDbContext> factory,
        AppSessionService session,
        ILogger<LogService> logger)
    {
        _factory = factory;
        _session = session;
        _logger = logger;
    }

    /// <summary>
    /// Registra un cambio en la auditoría. El usuario se resuelve automáticamente
    /// desde la sesión actual; pásalo explícito solo si necesitas sobreescribirlo.
    /// Un fallo al auditar NO interrumpe la operación: se registra en el log de la app.
    /// </summary>
    public async Task RegistrarAsync(
        string tabla,
        string accion,
        string registroId,
        object? anterior = null,
        object? nuevo = null,
        string? usuario = null)
    {
        // Usuario centralizado: evita el bug de pasar null y unifica el origen.
        usuario = string.IsNullOrWhiteSpace(usuario)
            ? (_session.CurrentUser?.Email ?? "Sistema")
            : usuario;

        try
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
        catch (Exception ex)
        {
            // La auditoría no debe tumbar la operación de negocio, pero el fallo
            // sí debe quedar visible en el log de la aplicación.
            _logger.LogError(ex,
                "No se pudo registrar la auditoría {Accion} sobre {Tabla} #{RegistroId} por {Usuario}.",
                accion, tabla, registroId, usuario);
        }
    }
}
