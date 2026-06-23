using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Pulperia.Data;
using Pulperia.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// Logger de arranque: captura errores tempranos (antes de construir el host).
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando Pulpería...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog como proveedor de logging. Lee de appsettings (sección "Serilog")
    // si existe, y siempre escribe a consola y a archivo rotativo diario.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/pulperia-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<WeatherForecastService>();

    // SQLite
    builder.Services.AddDbContextFactory<PulperiaDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Supabase
    var supabaseUrl = builder.Configuration["Supabase:Url"];
    var supabaseKey = builder.Configuration["Supabase:AnonKey"];

    var supabasePublic = new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions { Schema = "public" });
    await supabasePublic.InitializeAsync();
    builder.Services.AddSingleton(supabasePublic);
    builder.Services.AddScoped<AppSessionService>();
    builder.Services.AddScoped<EmpleadoService>();
    builder.Services.AddScoped<LogService>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<PulperiaDbContext>();

            // Verifica si puede conectarse
            if (!db.Database.CanConnect())
            {
                app.Logger.LogWarning("No se pudo conectar a la base de datos. Intentando crearla y migrarla...");

                // Crea la DB y aplica migraciones
                db.Database.Migrate();

                app.Logger.LogInformation("Base de datos creada y migrada correctamente.");
            }
            else
            {
                app.Logger.LogInformation("La base de datos ya existe y está disponible.");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error al inicializar la base de datos.");
            throw;
        }
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Registra cada petición HTTP en una sola línea resumida.
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó de forma inesperada.");
}
finally
{
    Log.CloseAndFlush();
}
