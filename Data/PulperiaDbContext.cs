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

    public DbSet<Proveedor> Proveedores => Set<Proveedor>(); public DbSet<Venta> Venta => Set<Venta>();
}