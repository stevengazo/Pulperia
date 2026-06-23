# ⚙️ Configuración y despliegue

Guía para configurar credenciales, entornos y publicar **Pulpería**.

---

## 1. Claves de configuración

| Clave | Descripción | Ejemplo |
|-------|-------------|---------|
| `ConnectionStrings:DefaultConnection` | Conexión a SQL Server (EF Core). | `Server=HOST;Database=Pulperia;User Id=USR;Password=***;TrustServerCertificate=True;` |
| `Supabase:Url` | URL del proyecto de Supabase. | `https://xxxx.supabase.co` |
| `Supabase:AnonKey` | Clave pública/anónima de Supabase. | `sb_publishable_xxxx` |
| `Logging:LogLevel:Default` | Nivel de log por defecto. | `Information` |
| `AllowedHosts` | Hosts permitidos. | `*` |

El repositorio incluye [`appsettings.Example.json`](../appsettings.Example.json) como plantilla **versionada y sin secretos**. El `appsettings.json` real está **ignorado por git** (`.gitignore`), así que crea el tuyo copiándolo:

```bash
cp appsettings.Example.json appsettings.json
```

Plantilla mínima (igual que `appsettings.Example.json`, **sin valores reales**):

```json
{
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=Pulperia;User Id=USUARIO;Password=CONTRASEÑA;TrustServerCertificate=True;"
  },
  "Supabase": {
    "Url": "https://TU_PROYECTO.supabase.co",
    "AnonKey": "TU_ANON_KEY"
  }
}
```

---

## 2. Gestión de secretos 🔐

> ⛔ **Nunca** subas credenciales reales al repositorio. El historial de git las conserva aunque se borren después.

### Desarrollo — User Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=Pulperia;..."
dotnet user-secrets set "Supabase:Url"     "https://TU_PROYECTO.supabase.co"
dotnet user-secrets set "Supabase:AnonKey" "TU_ANON_KEY"
```

Los secretos se guardan fuera del proyecto (no se versionan) y sobrescriben `appsettings.json`.

### Producción — Variables de entorno

Usa `__` (doble guion bajo) como separador de sección:

```bash
export ConnectionStrings__DefaultConnection="Server=...;Database=Pulperia;..."
export Supabase__Url="https://TU_PROYECTO.supabase.co"
export Supabase__AnonKey="TU_ANON_KEY"
export ASPNETCORE_ENVIRONMENT="Production"
```

### Orden de precedencia (ASP.NET Core)

```
Variables de entorno  >  User Secrets (Dev)  >  appsettings.{Environment}.json  >  appsettings.json
```

---

## 3. Variables de ejecución

| Variable | Descripción | Valor |
|----------|-------------|-------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución. | `Development` / `Production` |
| `ASPNETCORE_URLS` | URLs de escucha. | `https://localhost:7164;http://localhost:5221` |

Los perfiles de arranque están en [`Properties/launchSettings.json`](../Properties/launchSettings.json).

---

## 4. Base de datos

La app intenta verificar y migrar la BD al arrancar (`Program.cs`). De forma manual:

```bash
# Instalar la herramienta (una vez)
dotnet tool install --global dotnet-ef

# Aplicar migraciones
dotnet ef database update
```

> Requiere una instancia de SQL Server accesible con la cadena de conexión configurada.

---

## 5. Supabase

1. Crea un proyecto en [supabase.com](https://supabase.com/).
2. En **Project Settings → API** copia la **URL** y la **anon/publishable key**.
3. Habilita **Email Auth** y crea al menos un usuario.
4. Crea la tabla `Empleados` con las columnas descritas en [`DATA-MODEL.md`](DATA-MODEL.md#empleado-supabase).

---

## 6. Logging (Serilog)

El logging usa **Serilog** (configurado en [`Program.cs`](../Program.cs)) con dos destinos:

| Destino | Detalle |
|---------|---------|
| 🖥️ **Consola** | Salida en la terminal durante la ejecución. |
| 📄 **Archivo** | `logs/pulperia-YYYYMMDD.log`, **rotación diaria**, se conservan los últimos **30** días. |

- La carpeta `logs/` está **ignorada por git**.
- Los niveles mínimos se definen en código: `Information` global, `Warning` para `Microsoft*`. Puedes sobreescribirlos añadiendo una sección `"Serilog"` en `appsettings.json` (se lee con `ReadFrom.Configuration`). Ejemplo:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": { "Microsoft": "Warning", "System": "Warning" }
    }
  }
}
```

- Cada petición HTTP se resume en una línea vía `UseSerilogRequestLogging()`.
- En el código, inyecta `ILogger<T>` y usa logging estructurado: `Logger.LogError(ex, "... {VentaId}", id)`.

---

## 7. Publicación

```bash
# Framework-dependent
dotnet publish -c Release -o ./publish

# Self-contained (Windows x64)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

Despliega el contenido de `./publish` en el host (IIS, servicio systemd, contenedor, etc.) y define las variables de entorno con las credenciales.

### Lista de verificación de producción ✅

- [ ] Credenciales fuera de `appsettings.json` (variables de entorno).
- [ ] Credenciales **rotadas** si alguna vez se versionaron.
- [ ] `ASPNETCORE_ENVIRONMENT=Production`.
- [ ] HTTPS y certificado válido (la app usa `UseHsts` + `UseHttpsRedirection`).
- [ ] Base de datos migrada y respaldada.
- [ ] Usuarios de Supabase creados.
