[← Volver al índice](README.md)

# Guía de pruebas unitarias

Este documento explica **a fondo** cómo están hechas las pruebas unitarias de
PulpePOS: qué prueban, con qué herramientas, por qué se tomaron ciertas
decisiones y cómo agregar pruebas nuevas. Está dirigido a **desarrolladores**.

> Referencia rápida de uso: [`Pulperia.Tests/README.md`](../Pulperia.Tests/README.md).
> Aquí encontrarás la explicación detallada.

---

## 1. ¿Qué es una prueba unitaria y qué buscamos?

Una **prueba unitaria** ejecuta un fragmento pequeño de código (una función, un
método, una clase) de forma aislada y comprueba que su resultado sea el esperado.
Si alguien cambia el código y rompe el comportamiento, la prueba **falla** y avisa
antes de que el error llegue al usuario.

En este proyecto priorizamos probar la **lógica de negocio con valor real**:

- El **cobro de una venta** (`VentaService`): totales, cambio, estado de pago y —
  sobre todo — el **descuento correcto de inventario**, incluso ante errores.
- La **auditoría** (`LogService`): que registre bien y que **nunca tumbe** la
  operación principal si falla.
- Los **cálculos de dinero** (propiedades calculadas de los modelos).

No probamos todo por probar: buscamos los puntos donde un error costaría **dinero o
datos**.

---

## 2. Herramientas usadas

| Herramienta | Para qué |
|-------------|----------|
| **xUnit** | Framework de pruebas (define los casos con `[Fact]` y `[Theory]`). |
| **SQLite en memoria** | Base de datos real pero efímera para probar el código que usa Entity Framework. |
| **coverlet** | Mide la cobertura de código (qué porcentaje se ejecutó). |
| **GitHub Actions** | Ejecuta todo automáticamente en cada cambio. |

### `[Fact]` vs `[Theory]`

- **`[Fact]`** — una prueba con un único escenario fijo.
- **`[Theory]` + `[InlineData(...)]`** — la misma prueba ejecutada con **varios
  juegos de datos**. Ejemplo real de `ModelTests.cs`:

```csharp
[Theory]
[InlineData(1, 500, 500)]     // 1 × 500 = 500
[InlineData(3, 500, 1500)]    // 3 × 500 = 1500
[InlineData(0, 999, 0)]       // sin cantidad, subtotal 0
public void DetalleVenta_Subtotal_multiplica_cantidad_por_precio(
    int cantidad, decimal precio, decimal esperado)
{
    var detalle = new DetalleVenta { Cantidad = cantidad, PrecioUnitarioHistorico = precio };
    Assert.Equal(esperado, detalle.Subtotal);
}
```

Esa única prueba se ejecuta **3 veces**, una por cada `[InlineData]`.

---

## 3. El patrón AAA (Arrange – Act – Assert)

Todas las pruebas siguen la misma estructura de tres pasos:

1. **Arrange (preparar):** se crea el escenario — base de datos, servicios, datos.
2. **Act (actuar):** se ejecuta el método que se está probando.
3. **Assert (verificar):** se comprueba que el resultado sea el esperado.

```csharp
[Fact]
public async Task ProcesarAsync_falla_si_el_carrito_esta_vacio()
{
    // Arrange
    using var factory = new SqliteTestFactory();
    var service = TestHelpers.CreateVentaService(factory);

    // Act
    var resultado = await service.ProcesarAsync(
        new List<DetalleVenta>(), "Efectivo", 0, null, "test");

    // Assert
    Assert.False(resultado.Exito);
    Assert.Equal("El carrito está vacío.", resultado.Error);
}
```

---

## 4. El problema de probar código que usa base de datos

`VentaService` y `LogService` no son funciones puras: **leen y escriben en la base
de datos** mediante Entity Framework. Para probarlos necesitamos una base de datos
que:

- No requiera instalar SQL Server ni conexión de red.
- Sea **rápida** y **descartable** (cada prueba empieza limpia).
- Se comporte **como una base real** (con transacciones).

### ¿Por qué SQLite y no el proveedor *InMemory* de EF Core?

EF Core trae un proveedor "InMemory" muy cómodo, pero tiene un límite grave para
nosotros: **no soporta transacciones ni `ExecuteUpdateAsync`**, y `VentaService`
usa justamente eso para descontar stock de forma segura:

```csharp
await using var tx = await db.Database.BeginTransactionAsync();   // ❌ InMemory no lo soporta
...
var filas = await db.Producto
    .Where(p => p.Id == item.ProductoId && p.StockActual >= item.Cantidad)
    .ExecuteUpdateAsync(...);                                     // ❌ InMemory no lo soporta
```

Si usáramos InMemory, la prueba pasaría "en falso" (ignorando la parte más
importante) o reventaría. **SQLite en memoria sí soporta transacciones y
`ExecuteUpdate`**, y se comporta como una base relacional real → pruebas fiables.

---

## 5. La infraestructura de prueba

### 5.1 `SqliteTestFactory` — la base de datos efímera

Archivo: [`Pulperia.Tests/Infrastructure/SqliteTestFactory.cs`](../Pulperia.Tests/Infrastructure/SqliteTestFactory.cs)

El código de la app no crea sus `DbContext` directamente: los pide a una **fábrica**
(`IDbContextFactory<PulperiaDbContext>`). En las pruebas le damos una fábrica que
entrega contextos apuntando a una base **SQLite en memoria**:

```csharp
public sealed class SqliteTestFactory : IDbContextFactory<PulperiaDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<PulperiaDbContext> _options;

    public SqliteTestFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();                       // (1) mantener la conexión abierta

        _options = new DbContextOptionsBuilder<PulperiaDbContext>()
            .UseSqlite(_connection)               // (2) todos los contextos usan ESTA conexión
            .Options;

        using var ctx = new PulperiaDbContext(_options);
        ctx.Database.EnsureCreated();             // (3) crea las tablas desde el modelo de EF
    }

    public PulperiaDbContext CreateDbContext() => new(_options);
    public void Dispose() => _connection.Dispose();   // (4) al terminar, se destruye la base
}
```

**El detalle clave (1):** una base `:memory:` de SQLite **existe solo mientras su
conexión esté abierta**. Si cada contexto abriera y cerrara su propia conexión,
cada uno vería una base distinta y vacía. Por eso mantenemos **una sola conexión
abierta** durante toda la prueba (2) y todos los contextos la comparten → todos ven
los mismos datos. Al hacer `Dispose` (4), la base desaparece: la siguiente prueba
empieza desde cero.

`EnsureCreated()` (3) construye el esquema (tablas, columnas, relaciones)
directamente desde las clases del modelo, sin necesidad de migraciones.

> Como cada prueba crea su propia `SqliteTestFactory` (con `using`), las pruebas
> están **aisladas**: no comparten datos ni se afectan entre sí.

### 5.2 `TestHelpers` — construir servicios sin Supabase

Archivo: [`Pulperia.Tests/Infrastructure/TestHelpers.cs`](../Pulperia.Tests/Infrastructure/TestHelpers.cs)

`LogService` necesita un `AppSessionService` (para saber qué usuario hizo el
cambio), y ese servicio normalmente depende de **Supabase**. Pero en la ruta que
probamos, `LogService` solo **lee** `session.CurrentUser`, que por defecto es
`null`. Aprovechamos eso para construir el servicio con dependencias nulas, sin
levantar Supabase:

```csharp
public static LogService CreateLogService(IDbContextFactory<PulperiaDbContext> factory)
{
    // El constructor de AppSessionService solo GUARDA sus dependencias; no las usa
    // mientras CurrentUser sea null. Por eso podemos pasar null! sin problema.
    var session = new AppSessionService(
        supabase: null!, serviceProvider: null!,
        logger: NullLogger<AppSessionService>.Instance);

    return new LogService(factory, session, NullLogger<LogService>.Instance);
}
```

`NullLogger<T>.Instance` es un logger que **descarta** todo lo que recibe: útil en
pruebas porque no queremos ruido en consola ni dependencias de configuración.

`TestHelpers` también ofrece atajos para **sembrar** datos (`SeedProducto`), armar
líneas de carrito (`LineaCarrito`) y leer el stock actual (`GetStock`).

---

## 6. Recorrido por las pruebas

### 6.1 `VentaServiceTests` — la más importante

Prueba `VentaService.ProcesarAsync`, que registra una venta de forma
**transaccional**. Casos:

| Prueba | Qué verifica |
|--------|--------------|
| `...carrito_esta_vacio` | Devuelve fallo con el mensaje correcto y no toca la base. |
| `...efectivo_descuenta_stock_y_calcula_cambio` | Total = 1500, cambio = 500, `Pagado = true`, **stock baja de 10 a 7**, se crea el detalle y el log de auditoría. |
| `...fiada_queda_pendiente_y_sin_cambio` | `Pagado = false`, `MontoRecibido = Total`, cambio 0; el stock **igual se descuenta**. |
| `...no_efectivo_no_genera_cambio` | Con tarjeta, aunque se pase un monto alto, `MontoRecibido = Total` y cambio 0. |
| `...stock_insuficiente_revierte_todo` | Si no alcanza el stock, **se revierte todo**: no queda venta, ni detalle, y el stock no cambia. |

**Cómo se prueba la reversión (lo más interesante).** `ProcesarAsync` descuenta el
stock con una *guarda* que solo actualiza la fila si todavía hay existencias:

```csharp
var filas = await db.Producto
    .Where(p => p.Id == item.ProductoId && p.StockActual >= item.Cantidad)
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockActual, p => p.StockActual - item.Cantidad));

if (filas == 0)          // nadie cumplió la condición → no había stock
{
    await tx.RollbackAsync();   // se deshace TODO (incluida la venta ya insertada)
    return ResultadoVenta.Fallo("Stock insuficiente para ...");
}
```

La prueba siembra un producto con **2 unidades**, intenta vender **5**, y luego
comprueba con una consulta nueva que **no se guardó nada** y que el stock sigue en
2. Esto solo es verificable porque usamos SQLite (con transacciones reales); es la
razón concreta por la que no se usó el proveedor InMemory.

```csharp
// Assert de la reversión
Assert.False(resultado.Exito);
Assert.Contains("Stock insuficiente", resultado.Error);
Assert.Equal(0, await db.Venta.CountAsync());          // no se guardó la venta
Assert.Equal(0, await db.DetalleVenta.CountAsync());   // ni el detalle
Assert.Equal(2, TestHelpers.GetStock(factory, productoId)); // stock intacto
```

### 6.2 `LogServiceTests` — la auditoría

Prueba `LogService.RegistrarAsync`. Puntos cubiertos:

- **Guarda correctamente** tabla, acción, id y usuario, y serializa los valores
  `anterior`/`nuevo` a **JSON** (se verifica buscando fragmentos como
  `"StockActual":12` en el texto guardado).
- **Resuelve el usuario:** usa el que se pasa explícitamente o, si no hay ninguno y
  no hay sesión, `"Sistema"`.
- **Tolerancia a fallos:** si la base falla, **no lanza excepción**. Se prueba con
  una fábrica que revienta a propósito:

```csharp
private sealed class ThrowingFactory : IDbContextFactory<PulperiaDbContext>
{
    public PulperiaDbContext CreateDbContext() =>
        throw new InvalidOperationException("Fallo simulado de base de datos.");
}
...
var excepcion = await Record.ExceptionAsync(() => log.RegistrarAsync("Producto", "INSERT", "1"));
Assert.Null(excepcion);   // el fallo se traga; la auditoría nunca tumba el negocio
```

`Record.ExceptionAsync(...)` ejecuta el código y **captura** la excepción si la
hubiera (en vez de dejar que rompa la prueba). Si devuelve `null`, es que no se
lanzó ninguna → comportamiento correcto.

### 6.3 `ModelTests` y `ResultadoVentaTests` — cálculos y contrato

Pruebas rápidas y sin base de datos:

- `ModelTests` fija los cálculos de dinero: `DetalleVenta.Subtotal`,
  `DetalleCompra.Total`, `CompraInventario.TotalCalculado` (incluyendo el caso de
  compra sin detalles → total 0).
- `ResultadoVentaTests` verifica que `ResultadoVenta.Ok(id)` y
  `ResultadoVenta.Fallo(msg)` dejen el objeto coherente (un éxito nunca trae error,
  y un fallo nunca trae id).

---

## 7. Cómo ejecutar las pruebas

Desde la raíz del repositorio:

```bash
# Todas las pruebas
dotnet test Pulperia.sln

# Solo el proyecto de pruebas
dotnet test Pulperia.Tests/Pulperia.Tests.csproj

# Con reporte de cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

### Cómo leer el resultado

```
Correctas! - Con error: 0, Superado: 19, Omitido: 0, Total: 19, Duración: 1 s
```

- **Superado** = pruebas que pasaron.
- **Con error** = pruebas que fallaron (aparecerá el detalle de cuál y por qué).
- **Omitido** = pruebas marcadas para saltarse.

---

## 8. Integración continua (GitHub Actions)

Archivo: [`.github/workflows/ci.yml`](../.github/workflows/ci.yml)

En cada *push* a `main`/`Quality` y en cada *pull request* a `main`, GitHub ejecuta
automáticamente:

1. **Checkout** del código.
2. Instala el **SDK de .NET 8**.
3. **Restaura** los paquetes NuGet (con caché para ir más rápido).
4. **Compila** la solución en `Release`.
5. **Ejecuta las pruebas** con cobertura.
6. **Publica** los resultados como artefacto descargable.

Si alguna prueba falla, el workflow se marca en **rojo** y el *pull request* queda
señalado, evitando integrar código roto.

> El CI corre en **Linux**, donde no existe el bloqueo de Smart App Control que
> puede aparecer al ejecutar en Windows (ver §10).

---

## 9. Cómo agregar una prueba nueva

1. Crea o abre un archivo `*Tests.cs` en `Pulperia.Tests/`.
2. Escribe un método `public` con `[Fact]` (o `[Theory]` para varios datos) y un
   **nombre descriptivo** de lo que verifica.
3. Sigue el patrón **Arrange – Act – Assert**.
4. Si necesitas base de datos, usa `SqliteTestFactory` y los ayudantes de
   `TestHelpers`.
5. Ejecuta `dotnet test` y confirma que pasa.

Ejemplo mínimo:

```csharp
[Fact]
public async Task Mi_nueva_regla_de_negocio_funciona()
{
    // Arrange
    using var factory = new SqliteTestFactory();
    var productoId = TestHelpers.SeedProducto(factory, stock: 8);
    var service = TestHelpers.CreateVentaService(factory);

    // Act
    var resultado = await service.ProcesarAsync(
        new[] { TestHelpers.LineaCarrito(productoId, 1, 500m) },
        "Efectivo", 500m, null, "test");

    // Assert
    Assert.True(resultado.Exito);
    Assert.Equal(7, TestHelpers.GetStock(factory, productoId));
}
```

### Buenas prácticas
- **Un concepto por prueba.** Si el nombre necesita "y", quizá son dos pruebas.
- **Nombres que expliquen el escenario y el resultado esperado.**
- **Sin dependencias entre pruebas:** cada una crea su propia base con `using`.
- **Prueba también los errores**, no solo el camino feliz (carrito vacío, stock
  insuficiente, fallos de base…).

---

## 10. Solución de problemas

**Al ejecutar `dotnet test` en Windows aparece `0x800711C7` / "Control de
aplicaciones bloqueó este archivo".**
No es un fallo de las pruebas, sino de **Smart App Control** de Windows bloqueando
los ensamblados compilados sin firma. Ver
[docs/10-preguntas-frecuentes.md](10-preguntas-frecuentes.md#problemas-tecnicos).
En GitHub Actions (Linux) no ocurre.

**`error CS0246: no se encontró 'Fact' / 'InlineData'`.**
Falta el `using Xunit;` en el archivo de prueba, o el proyecto principal está
incluyendo por error los `.cs` de pruebas (ya resuelto excluyendo `Pulperia.Tests/`
en `Pulperia.csproj`).

**Una prueba con base de datos ve la base vacía.**
Asegúrate de usar la **misma** `SqliteTestFactory` en todo el test y de no cerrar la
conexión antes de tiempo (por eso `SqliteTestFactory` mantiene la conexión abierta
hasta su `Dispose`).

---

## 11. Qué no se prueba (y por qué)

- **`EmpleadoService` y `AppSessionService`** dependen directamente del SDK de
  **Supabase** (datos y autenticación remotos). Probarlos de verdad requiere
  **pruebas de integración** contra un Supabase real o dobles del SDK, fuera del
  alcance de las pruebas unitarias.
- **Componentes Blazor (`.razor`)** requieren **bUnit** para renderizarse en
  pruebas. La lógica de negocio importante ya se extrajo a `VentaService`, que sí se
  prueba aquí; los componentes quedan como candidatos a pruebas de UI más adelante.

---

[← Volver al índice](README.md)
