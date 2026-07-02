using Microsoft.EntityFrameworkCore;
using Pulperia.Models;
using Supabase;


namespace Pulperia.Data;


public class PulperiaDbContext : DbContext
{
    public PulperiaDbContext(DbContextOptions<PulperiaDbContext> options) : base(options)
    {

    }

    public DbSet<Categoria> Categoria => Set<Categoria>();
    public DbSet<CompraInventario> CompraInventario => Set<CompraInventario>();
    public DbSet<DetalleCompra> DetalleCompra => Set<DetalleCompra>();
    public DbSet<DetalleVenta> DetalleVenta => Set<DetalleVenta>();
    public DbSet<Producto> Producto => Set<Producto>();
    public DbSet<RolSystem> Roles => Set<RolSystem>();
    public DbSet<RolUser> RolUsers => Set<RolUser>();
    public DbSet<LogCambio> LogsCambios => Set<LogCambio>();

    public DbSet<Proveedor> Proveedores => Set<Proveedor>(); public DbSet<Venta> Venta => Set<Venta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CategoriaId lleva [Required] para validar el formulario, pero la columna
        // admite NULL (hay productos sin categoría). Fluent API tiene prioridad sobre
        // la anotación en el mapeo, así EF genera lectura null-safe y no lanza
        // SqlNullValueException al materializar esas filas.
        modelBuilder.Entity<Producto>()
            .Property(p => p.CategoriaId)
            .IsRequired(false);
    }
}