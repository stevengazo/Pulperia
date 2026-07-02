# Pulperia.Tests — Pruebas unitarias

Suite de pruebas unitarias del proyecto **PulpePOS**, escritas con **xUnit** y
ejecutadas sobre **SQLite en memoria** para la lógica que toca base de datos.

## Cómo ejecutarlas

Desde la raíz del repositorio:

```bash
dotnet test Pulperia.sln
```

O solo este proyecto:

```bash
dotnet test Pulperia.Tests/Pulperia.Tests.csproj
```

Con cobertura de código:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Qué se prueba

| Archivo | Área | Casos cubiertos |
|---------|------|-----------------|
| `VentaServiceTests.cs` | **Lógica de ventas** (`VentaService.ProcesarAsync`) | Carrito vacío · venta en efectivo (total, cambio, pagado, descuento de stock, detalle y auditoría) · venta fiada (pendiente, sin cambio) · métodos no-efectivo (sin cambio) · **stock insuficiente → reversión total** |
| `LogServiceTests.cs` | **Auditoría** (`LogService.RegistrarAsync`) | Guarda el log con usuario explícito y serializa a JSON · usa `"Sistema"` sin sesión · deja nulos los valores no provistos · **no lanza excepción si falla la base** |
| `ModelTests.cs` | **Propiedades calculadas** | `DetalleVenta.Subtotal` · `DetalleCompra.Total` · `CompraInventario.TotalCalculado` (con y sin detalles) |
| `ResultadoVentaTests.cs` | **Contrato de resultado** | Fábricas `ResultadoVenta.Ok` y `.Fallo` |

## Diseño de las pruebas

- **`Infrastructure/SqliteTestFactory.cs`** — fábrica de `PulperiaDbContext` sobre
  SQLite `:memory:`. Se usa SQLite (y no el proveedor *InMemory* de EF Core)
  porque el código bajo prueba usa **transacciones** y **`ExecuteUpdateAsync`**,
  que *InMemory* no soporta. La conexión se mantiene abierta durante toda la
  prueba para que la base en memoria persista entre contextos.
- **`Infrastructure/TestHelpers.cs`** — construye los servicios reales
  (`VentaService`, `LogService`) con dependencias mínimas (sin Supabase ni red) y
  siembra datos de prueba.

## Qué **no** se prueba (y por qué)

- **`EmpleadoService`** y **`AppSessionService`** dependen directamente del cliente
  de **Supabase** (autenticación y datos remotos). No son unit-testeables de forma
  fiable sin infraestructura de integración o dobles de prueba del SDK de Supabase;
  se recomienda cubrirlos con **pruebas de integración** aparte.
- Los **componentes Blazor** (`.razor`) requieren un framework como **bUnit** para
  probarse; su lógica de negocio relevante ya está extraída a `VentaService`, que
  sí se prueba aquí.

## Integración continua

El workflow [`.github/workflows/ci.yml`](../.github/workflows/ci.yml) restaura,
compila en `Release` y ejecuta estas pruebas en cada *push* a `main`/`Quality` y en
cada *pull request* a `main`.

> **Nota (solo Windows local):** si al ejecutar `dotnet test` aparece el error
> `0x800711C7` ("Control de aplicaciones bloqueó este archivo"), es **Smart App
> Control** de Windows bloqueando los ensamblados compilados, no un fallo de las
> pruebas. Ver [docs/10-preguntas-frecuentes.md](../docs/10-preguntas-frecuentes.md).
> En GitHub Actions (Linux) esto no ocurre.
