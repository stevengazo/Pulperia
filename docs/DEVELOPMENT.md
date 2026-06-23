# 👩‍💻 Guía de desarrollo

Convenciones y prácticas para trabajar en **Pulpería**.

---

## 1. Entorno

```bash
dotnet restore        # Restaurar paquetes
dotnet watch run      # Ejecutar con hot reload
dotnet build          # Compilar
```

Editores recomendados: **Visual Studio 2022 (17.8+)** o **VS Code** con la extensión *C# Dev Kit*. El proyecto incluye `.vscode/launch.json` y `tasks.json`.

---

## 2. Convenciones

### Idioma
- **Dominio y UI en español** (`Producto`, `Venta`, `NuevaVenta`). Mantén el español en entidades, rutas y textos visibles.
- Código de infraestructura puede usar términos en inglés cuando es idiomático (`AppSessionService`, `LoginAsync`).

### Estructura
- Páginas enrutables → `Pages/<Área>/`.
- Componentes reutilizables → `Components/<Área>/`.
- Cada componente con estilos propios usa CSS aislado (`*.razor.css`).
- Modelos de dominio → `models/`.
- Lógica que no es de UI → `Services/`.

### Acceso a datos (EF Core)
✅ **Preferir** un contexto efímero por operación con la fábrica:

```csharp
await using var db = await DbFactory.CreateDbContextAsync();
var items = await db.Producto.AsNoTracking().ToListAsync();
```

✅ **Usar siempre las versiones asíncronas**: `ToListAsync`, `FirstOrDefaultAsync`, `AnyAsync`, `SaveChangesAsync`… (no bloquees el circuito de Blazor).

✅ **Operaciones que tocan varias tablas** → transacción:

```csharp
await using var tx = await db.Database.BeginTransactionAsync();
// ...
await tx.CommitAsync();
```

✅ **Actualizaciones con condición de carrera** (p. ej. stock) → descuento atómico:

```csharp
var filas = await db.Producto
    .Where(p => p.Id == id && p.StockActual >= cantidad)
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockActual, p => p.StockActual - cantidad));
if (filas == 0) { /* sin stock → rollback */ }
```

❌ Evitar: mantener un `DbContext` en un campo durante toda la vida del componente; llamadas síncronas a la BD; predecir Ids con `Max(Id)+1`.

---

## 3. Migraciones EF Core

```bash
# Crear una migración tras cambiar el modelo
dotnet ef migrations add DescripcionDelCambio

# Aplicar a la base de datos
dotnet ef database update

# Revertir a una migración anterior
dotnet ef database update NombreMigracionAnterior

# Eliminar la última migración (si no se aplicó)
dotnet ef migrations remove
```

Las migraciones se generan en [`Migrations/`](../Migrations/). Revisa el SQL generado antes de aplicarlo en producción.

---

## 4. Servicios y DI

Registra nuevos servicios en [`Program.cs`](../Program.cs):

```csharp
builder.Services.AddScoped<MiServicio>();
```

Guía de ciclo de vida:

| Tipo | Cuándo |
|------|--------|
| **Singleton** | Estado global sin dependencias por usuario (p. ej. `Supabase.Client`). |
| **Scoped** | Lo habitual en Blazor Server: una instancia por circuito/usuario. |
| **Transient** | Servicios sin estado y baratos de crear. |

> En Blazor Server, *scoped* equivale a «por circuito SignalR», no por petición HTTP.

---

## 5. Auditoría

Para registrar cambios usa `LogService`:

```csharp
await LogService.RegistrarAsync(
    tabla: "Producto",
    accion: "Editar",
    registroId: producto.Id.ToString(),
    usuario: Session.CurrentUser?.Email ?? "desconocido",
    anterior: estadoPrevio,
    nuevo: producto);
```

---

## 6. Generación de documentos

- **PDF** (tickets): `VentaPdfGenerator.Generar(venta, empleado)` → `byte[]`. Requiere `venta.DetalleVentas` cargado.
- **Excel**: ClosedXML (ver componentes que exportan reportes).
- La descarga al navegador se hace con [`wwwroot/download.js`](../wwwroot/download.js) vía JS interop.

---

## 7. Estilo de commits

Mensajes cortos y descriptivos, en español, en imperativo:

```
Fix Login Page
Agrega transacción a la venta
CSS Separation
```

---

## 8. Checklist antes de un PR

- [ ] `dotnet build` sin errores.
- [ ] Si cambiaste el modelo, generaste y probaste la migración.
- [ ] No agregaste credenciales ni secretos al repo.
- [ ] Llamadas a la BD asíncronas; operaciones multi-tabla en transacción.
- [ ] Probaste el flujo afectado en ejecución (`dotnet run`).
