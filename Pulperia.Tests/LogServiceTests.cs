using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Pulperia.Data;
using Pulperia.Services;
using Pulperia.Tests.Infrastructure;
using Xunit;

namespace Pulperia.Tests;

/// <summary>
/// Pruebas de <see cref="LogService.RegistrarAsync"/>, el registro de auditoría.
///
/// Verifican tres comportamientos clave:
///  1. Guarda el cambio con los campos correctos y serializa a JSON los valores
///     anterior/nuevo.
///  2. Resuelve el usuario: usa el pasado explícitamente o, en su defecto,
///     "Sistema" cuando no hay sesión.
///  3. Es <b>tolerante a fallos</b>: si la persistencia falla, NO lanza excepción
///     (la auditoría nunca debe tumbar la operación de negocio).
/// </summary>
public class LogServiceTests
{
    [Fact]
    public async Task RegistrarAsync_guarda_el_log_con_usuario_explicito_y_json()
    {
        // Arrange
        using var factory = new SqliteTestFactory();
        var log = TestHelpers.CreateLogService(factory);

        // Act
        await log.RegistrarAsync(
            tabla: "Producto",
            accion: "INCREMENTO_INVENTARIO",
            registroId: "7",
            anterior: new { StockActual = 5 },
            nuevo: new { StockActual = 12, Cantidad = 7 },
            usuario: "cajero@pulperia.cr");

        // Assert
        using var db = factory.CreateDbContext();
        var entrada = await db.LogsCambios.SingleAsync();

        Assert.Equal("Producto", entrada.Tabla);
        Assert.Equal("INCREMENTO_INVENTARIO", entrada.Accion);
        Assert.Equal("7", entrada.RegistroId);
        Assert.Equal("cajero@pulperia.cr", entrada.Usuario);

        // Los valores se guardan como JSON.
        Assert.Contains("\"StockActual\":5", entrada.ValoresAnteriores);
        Assert.Contains("\"StockActual\":12", entrada.ValoresNuevos);
        Assert.Contains("\"Cantidad\":7", entrada.ValoresNuevos);
    }

    [Fact]
    public async Task RegistrarAsync_usa_Sistema_cuando_no_hay_usuario_ni_sesion()
    {
        // Arrange
        using var factory = new SqliteTestFactory();
        var log = TestHelpers.CreateLogService(factory); // sesión sin usuario

        // Act: no se pasa 'usuario'.
        await log.RegistrarAsync(
            tabla: "Categoria",
            accion: "INSERT",
            registroId: "1",
            nuevo: new { Id = 1, Nombre = "Bebidas" });

        // Assert
        using var db = factory.CreateDbContext();
        var entrada = await db.LogsCambios.SingleAsync();

        Assert.Equal("Sistema", entrada.Usuario);
    }

    [Fact]
    public async Task RegistrarAsync_deja_nulos_los_valores_cuando_no_se_pasan()
    {
        using var factory = new SqliteTestFactory();
        var log = TestHelpers.CreateLogService(factory);

        await log.RegistrarAsync(
            tabla: "Venta",
            accion: "DELETE",
            registroId: "99");

        using var db = factory.CreateDbContext();
        var entrada = await db.LogsCambios.SingleAsync();

        Assert.Null(entrada.ValoresAnteriores);
        Assert.Null(entrada.ValoresNuevos);
    }

    [Fact]
    public async Task RegistrarAsync_no_lanza_si_falla_la_persistencia()
    {
        // Arrange: una fábrica que revienta al crear el contexto simula un fallo
        // de base de datos.
        var factory = new ThrowingFactory();
        var session = new AppSessionService(null!, null!, NullLogger<AppSessionService>.Instance);
        var log = new LogService(factory, session, NullLogger<LogService>.Instance);

        // Act
        var excepcion = await Record.ExceptionAsync(() =>
            log.RegistrarAsync("Producto", "INSERT", "1"));

        // Assert: el fallo se traga (se registra internamente, no se propaga).
        Assert.Null(excepcion);
    }

    /// <summary>Fábrica que siempre falla, para probar la tolerancia a errores.</summary>
    private sealed class ThrowingFactory : IDbContextFactory<PulperiaDbContext>
    {
        public PulperiaDbContext CreateDbContext() =>
            throw new InvalidOperationException("Fallo simulado de base de datos.");
    }
}
