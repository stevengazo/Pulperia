using Microsoft.EntityFrameworkCore;
using Pulperia.Models;
using Supabase;


namespace Pulperia.Data ;


public class PulperiaDbContext : DbContext
{
    public PulperiaDbContext ( DbContextOptions<PulperiaDbContext> options ): base(options)
    {
        
    }

    public DbSet<Producto> Producto => Set<Producto>();
    public DbSet<Categoria> Categoria => Set<Categoria>();
    public DbSet<Cliente> Cliente => Set<Cliente>();
    public DbSet<DetalleVenta> DetalleVenta => Set<DetalleVenta> ();
    public DbSet<Venta> Venta => Set<Venta>();
    
}