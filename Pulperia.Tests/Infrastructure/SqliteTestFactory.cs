using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Pulperia.Data;

namespace Pulperia.Tests.Infrastructure;

/// <summary>
/// Fábrica de <see cref="PulperiaDbContext"/> respaldada por una base de datos
/// <b>SQLite en memoria</b>, pensada para pruebas unitarias.
///
/// ¿Por qué SQLite y no el proveedor InMemory de EF Core?
/// El código bajo prueba (<c>VentaService</c>) usa <c>transacciones</c> y
/// <c>ExecuteUpdateAsync</c>, que el proveedor InMemory NO soporta. SQLite sí
/// los soporta y se comporta como una base relacional real, dando pruebas más
/// fiables.
///
/// Detalle importante: una base ":memory:" existe solo mientras su conexión esté
/// abierta. Por eso mantenemos <see cref="_connection"/> abierta durante toda la
/// vida de la fábrica y todos los contextos comparten esa misma conexión, de modo
/// que ven los mismos datos. Al hacer <see cref="Dispose"/> se destruye la base.
/// </summary>
public sealed class SqliteTestFactory : IDbContextFactory<PulperiaDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<PulperiaDbContext> _options;

    public SqliteTestFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<PulperiaDbContext>()
            .UseSqlite(_connection)
            .Options;

        // Crea el esquema una sola vez a partir del modelo de EF.
        using var ctx = new PulperiaDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    /// <summary>Crea un contexto nuevo sobre la conexión compartida.</summary>
    public PulperiaDbContext CreateDbContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
