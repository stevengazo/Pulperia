using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Pulperia.Data;
using Pulperia.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
            Console.WriteLine("No se pudo conectar. Intentando crear la DB...");

            // Crea la DB y aplica migraciones
            db.Database.Migrate();

            Console.WriteLine("Base de datos creada.");
        }
        else
        {
            Console.WriteLine("La base de datos ya existe y está disponible.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();