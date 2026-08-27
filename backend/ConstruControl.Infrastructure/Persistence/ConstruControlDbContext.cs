using ConstruControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstruControl.Infrastructure.Persistence;

public class ConstruControlDbContext : DbContext
{
    public ConstruControlDbContext(DbContextOptions<ConstruControlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Obra> Obras => Set<Obra>();
    public DbSet<Material> Materiales => Set<Material>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<DetalleCompra> DetallesCompra => Set<DetalleCompra>();
    public DbSet<Consumo> Consumos => Set<Consumo>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<FotoObra> FotosObra => Set<FotoObra>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enums almacenados como texto (más legible en la BD que números)
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>()
            .HasMaxLength(30);

        modelBuilder.Entity<Obra>()
            .Property(o => o.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Compra>()
            .Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Notificacion>()
            .Property(n => n.Tipo)
            .HasConversion<string>()
            .HasMaxLength(30);

        // Precisión decimal explícita (evita warnings de EF Core con decimales)
        modelBuilder.Entity<Obra>().Property(o => o.Presupuesto).HasPrecision(18, 2);
        modelBuilder.Entity<Material>().Property(m => m.Stock).HasPrecision(18, 2);
        modelBuilder.Entity<Material>().Property(m => m.StockMinimo).HasPrecision(18, 2);
        modelBuilder.Entity<Material>().Property(m => m.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<Compra>().Property(c => c.Total).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleCompra>().Property(d => d.Cantidad).HasPrecision(18, 2);
        modelBuilder.Entity<DetalleCompra>().Property(d => d.PrecioUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<Consumo>().Property(c => c.Cantidad).HasPrecision(18, 2);

        // Email único
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Evitar borrado en cascada donde no corresponde (protege historial)
        modelBuilder.Entity<Compra>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Compras)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Consumo>()
            .HasOne(c => c.Responsable)
            .WithMany(u => u.ConsumosRegistrados)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
