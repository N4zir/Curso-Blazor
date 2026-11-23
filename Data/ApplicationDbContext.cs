using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Domain.Entities;

namespace CumbreSalonesCongresos.Data;

public class ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<EntidadConvenio> EntidadesConvenio => Set<EntidadConvenio>();
    public DbSet<Salon> Salones => Set<Salon>();
    public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<CronogramaItem> CronogramaItems => Set<CronogramaItem>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<ComentarioEvento> Comentarios => Set<ComentarioEvento>();
    public DbSet<Promocion> Promociones => Set<Promocion>();
    public DbSet<CasoExito> CasosExito => Set<CasoExito>();
    public DbSet<GaleriaItem> Galeria => Set<GaleriaItem>();
    public DbSet<Participante> Participantes => Set<Participante>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Cliente>().HasIndex(x => x.UserId);
        builder.Entity<Empleado>().HasIndex(x => x.UserId).IsUnique();
        builder.Entity<Salon>().Property(x => x.TarifaBase).HasPrecision(18, 2);
        builder.Entity<Cotizacion>().Property(x => x.MontoEstimado).HasPrecision(18, 2);
        builder.Entity<Cotizacion>().HasIndex(x => x.Numero).IsUnique();
        builder.Entity<Reserva>().Property(x => x.Monto).HasPrecision(18, 2);
        builder.Entity<Reserva>().HasIndex(x => new { x.SalonId, x.Inicio, x.Fin, x.Estado });
        builder.Entity<Evento>().Property(x => x.Presupuesto).HasPrecision(18, 2);
        builder.Entity<Contrato>().Property(x => x.MontoTotal).HasPrecision(18, 2);
        builder.Entity<Contrato>().HasIndex(x => x.Numero).IsUnique();
        builder.Entity<Factura>().Property(x => x.Monto).HasPrecision(18, 2);
        builder.Entity<Pago>().Property(x => x.Monto).HasPrecision(18, 2);

        builder.Entity<Reserva>()
            .HasOne(r => r.Evento)
            .WithOne(e => e.Reserva)
            .HasForeignKey<Evento>(e => e.ReservaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Contrato>()
            .HasOne(c => c.Evento)
            .WithMany(e => e.Contratos)
            .HasForeignKey(c => c.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ComentarioEvento>()
            .HasOne(c => c.Evento)
            .WithMany(e => e.Comentarios)
            .HasForeignKey(c => c.EventoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
}
