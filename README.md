# Pulpería

Sistema de punto de venta e inventario para una **pulpería** (tienda de abarrotes), construido con **Blazor Server** sobre **.NET 8**.

Permite gestionar productos, inventario, compras a proveedores, ventas, empleados y un registro de auditoría de cambios. La autenticación y la gestión de empleados se apoyan en **Supabase**, mientras que los datos transaccionales (productos, ventas, compras, etc.) se almacenan en **SQL Server** mediante **Entity Framework Core**.

---

## Características

- **Dashboard** con estadísticas generales de la pulpería.
- **Productos**: alta, edición, vista de detalle y control de stock (stock actual / stock mínimo).
- **Categorías** y **Proveedores**.
- **Inventario** y **Compras de inventario** (con detalle por producto).
- **Ventas**: nueva venta, edición, detalle e historial, con cálculo de método de pago, monto recibido y cambio.
- **Generación de tickets** de venta en **PDF** (QuestPDF).
- **Exportación a Excel** (ClosedXML).
- **Empleados**: gestión a través de Supabase.
- **Autenticación** con persistencia de sesión (7 días) vía Supabase Auth + almacenamiento local del navegador.
- **Roles de usuario** y **registro de auditoría** (LogCambios) de las operaciones.

---

## Stack tecnológico

| Área | Tecnología |
|------|------------|
| Framework | .NET 8 (`net8.0`) |
| UI | Blazor Server (Razor Components) |
| ORM | Entity Framework Core 8 |
| Base de datos | SQL Server |
| Auth / Empleados | Supabase (Auth + Postgrest) |
| PDF | QuestPDF |
| Excel | ClosedXML |

### Paquetes NuGet principales

- `Microsoft.EntityFrameworkCore` 8.0.11
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.11
- `Microsoft.EntityFrameworkCore.Sqlite` 8.0.11
- `Microsoft.EntityFrameworkCore.Design` / `.Tools` 8.0.11
- `Supabase` 1.1.1
- `QuestPDF` 2026.5.0
- `ClosedXML` 0.105.0

---

## Estructura del proyecto

```
Pulperia/
├── App.razor                  # Componente raíz de la app Blazor
├── Program.cs                 # Punto de entrada, configuración de servicios y middleware
├── _Imports.razor             # Usings globales para los componentes Razor
├── appsettings.json           # Configuración (conexión SQL, credenciales Supabase)
├── Pulperia.csproj            # Definición del proyecto y dependencias
├── Pulperia.sln               # Solución de Visual Studio
│
├── Components/                # Componentes reutilizables
│   ├── Categories/            #   AddCategory
│   ├── CompraInventario/      #   AddCompraInventario
│   ├── Products/              #   AddProduct
│   ├── Proveedor/             #   AddProveedor
│   ├── Shared/                #   DateRangePicker
│   ├── Users/                 #   UserList
│   └── Venta/                 #   GenerarTicket
│
├── Pages/                     # Páginas enrutables (@page)
│   ├── Auth/                  #   LoginPage, UserValidator
│   ├── Empleado/              #   Empleado, Empleados
│   ├── Inventario/            #   Inventario
│   ├── Producto/              #   Products, EditProduct, ViewProduct
│   ├── Ventas/                #   Ventas, NuevaVenta, Editar-Venta, Venta-info
│   ├── Settings/              #   SettingsPage
│   ├── logs/                  #   LogsPage
│   ├── Index.razor            #   Dashboard
│   ├── _Host.cshtml           #   Página host de Blazor Server
│   └── _Layout.cshtml
│
├── Services/                  # Lógica de negocio / servicios
│   ├── AppSessionService.cs   #   Sesión y autenticación (Supabase)
│   ├── EmpleadoService.cs     #   Gestión de empleados
│   ├── LogService.cs          #   Registro de auditoría de cambios
│   └── VentaPdfGenerator.cs   #   Generación de tickets PDF
│
├── Data/                      # Acceso a datos
│   ├── PulperiaDbContext.cs   #   DbContext de EF Core
│   └── WeatherForecast*.cs    #   (plantilla por defecto)
│
├── models/                    # Modelos de dominio
│   ├── Producto.cs            ├── Venta.cs        ├── DetalleVenta.cs
│   ├── Categoria.cs           ├── CompraInventario.cs ├── DetalleCompra.cs
│   ├── Proveedor.cs           ├── ProductoCosto.cs
│   ├── Empleado.cs            ├── RolSystem.cs    ├── RolUser.cs
│   └── LogCambio.cs
│
├── Migrations/                # Migraciones de EF Core
├── Shared/                    # Layout y navegación (MainLayout, NavMenu)
├── Properties/                # launchSettings.json
└── wwwroot/                   # Recursos estáticos (CSS, Bootstrap, open-iconic, JS)
```

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** accesible (local o remoto)
- Una cuenta y proyecto de [**Supabase**](https://supabase.com/) (para autenticación y empleados)
- Opcional: Visual Studio 2022 (17.5+) o VS Code

---

## Instalación

1. **Clonar el repositorio**

   ```bash
   git clone <url-del-repositorio>
   cd Pulperia
   ```

2. **Restaurar dependencias**

   ```bash
   dotnet restore
   ```

3. **Configurar variables** (ver sección [Configuración](#configuración)).

4. **Aplicar las migraciones / crear la base de datos**

   La aplicación intenta crear y migrar la base de datos automáticamente al arrancar
   (ver `Program.cs`). También puedes hacerlo manualmente:

   ```bash
   dotnet ef database update
   ```

   > Requiere la herramienta `dotnet-ef`. Si no la tienes instalada:
   > ```bash
   > dotnet tool install --global dotnet-ef
   > ```

5. **Ejecutar la aplicación**

   ```bash
   dotnet run
   ```

   Por defecto la app queda disponible en:
   - https://localhost:7164
   - http://localhost:5221

---

## Configuración

La configuración se encuentra en [`appsettings.json`](appsettings.json). Para entornos de
desarrollo o producción puedes sobreescribirla con `appsettings.Development.json`,
variables de entorno o *user secrets*.

### Variables / claves de configuración

| Clave | Descripción | Ejemplo |
|-------|-------------|---------|
| `ConnectionStrings:DefaultConnection` | Cadena de conexión a SQL Server (EF Core). | `Server=192.168.1.2;Database=Pulperia;User Id=usuario;Password=****;TrustServerCertificate=True;` |
| `Supabase:Url` | URL del proyecto de Supabase. | `https://xxxxxxxx.supabase.co` |
| `Supabase:AnonKey` | Clave pública / anónima de Supabase. | `sb_publishable_xxxxxxxx` |
| `Logging:LogLevel:Default` | Nivel de log por defecto. | `Information` |
| `AllowedHosts` | Hosts permitidos. | `*` |

Ejemplo mínimo de `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
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

> ⚠️ **Seguridad:** evita versionar credenciales reales. Usa
> [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) en desarrollo
> o variables de entorno en producción, por ejemplo:
> ```bash
> dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;"
> dotnet user-secrets set "Supabase:AnonKey" "..."
> ```
> Las variables de entorno usan `__` como separador de sección, p. ej.
> `ConnectionStrings__DefaultConnection` o `Supabase__Url`.

### Variables de entorno de ejecución

| Variable | Descripción | Valor |
|----------|-------------|-------|
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución. | `Development` / `Production` |
| `ASPNETCORE_URLS` | URLs de escucha (opcional). | `https://localhost:7164;http://localhost:5221` |

Los perfiles de arranque (URLs y entorno) se definen en
[`Properties/launchSettings.json`](Properties/launchSettings.json).

---

## Base de datos

El `PulperiaDbContext` expone los siguientes conjuntos de entidades:

- `Producto`, `Categoria`, `Proveedor`, `ProductoCosto`
- `CompraInventario`, `DetalleCompra`
- `Venta`, `DetalleVenta`
- `RolSystem` (`Roles`), `RolUser` (`RolUsers`)
- `LogCambio` (`LogsCambios`) — auditoría de cambios

Comandos útiles de EF Core:

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones a la base de datos
dotnet ef database update
```

> Nota: la gestión de **empleados** y la **autenticación** no usan SQL Server, sino
> tablas y servicios de **Supabase** (modelo `Empleado` mapeado a la tabla `Empleados`).

---

## Scripts / comandos habituales

```bash
dotnet restore           # Restaurar paquetes
dotnet build             # Compilar
dotnet run               # Ejecutar en desarrollo
dotnet watch run         # Ejecutar con recarga en caliente
dotnet publish -c Release -o ./publish   # Publicar para producción
```

---

## Licencia

Este proyecto se distribuye bajo la licencia **MIT**. Consulta el archivo [LICENSE](LICENSE).

Copyright (c) 2026 Steven Fabricio Gazo Maliaño
