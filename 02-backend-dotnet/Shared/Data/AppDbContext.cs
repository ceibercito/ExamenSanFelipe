using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace Shared.Data;

/// <summary>
/// DbContext compartido por los microservicios (patrón shared-database).
/// Mapea las 8 tablas del sistema de compras y ventas.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<CompraCab> CompraCab => Set<CompraCab>();
    public DbSet<CompraDet> CompraDet => Set<CompraDet>();
    public DbSet<VentaCab> VentaCab => Set<VentaCab>();
    public DbSet<VentaDet> VentaDet => Set<VentaDet>();
    public DbSet<MovimientoCab> MovimientoCab => Set<MovimientoCab>();
    public DbSet<MovimientoDet> MovimientoDet => Set<MovimientoDet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Precisión de decimales (evita warnings de EF Core)
        foreach (var prop in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal)))
        {
            prop.SetColumnType("decimal(18,2)");
        }

        // Relaciones cabecera-detalle
        modelBuilder.Entity<CompraDet>()
            .HasOne<CompraCab>().WithMany(c => c.Detalles).HasForeignKey(d => d.IdCompraCab);

        modelBuilder.Entity<VentaDet>()
            .HasOne<VentaCab>().WithMany(c => c.Detalles).HasForeignKey(d => d.IdVentaCab);

        modelBuilder.Entity<MovimientoDet>()
            .HasOne<MovimientoCab>().WithMany(c => c.Detalles).HasForeignKey(d => d.IdMovimientoCab);
    }
}
