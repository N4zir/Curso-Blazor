using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Application.Common;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Application.Services;

public class CotizacionService(ApplicationAuthDbContext db)
{
    public async Task<OperationResult<Cotizacion>> CrearSolicitudAsync(Cotizacion c)
    {
        if (c.FechaTentativaInicio.HasValue && c.FechaTentativaFin.HasValue && c.FechaTentativaFin <= c.FechaTentativaInicio)
            return OperationResult<Cotizacion>.Fail("La fecha fin debe ser posterior al inicio.");
        c.Numero = $"COT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100,999)}";
        c.Estado = EstadoCotizacion.Enviada;
        c.CreadoUtc = DateTime.UtcNow;
        db.Cotizaciones.Add(c);
        await db.SaveChangesAsync();
        return OperationResult<Cotizacion>.Success(c);
    }

    public Task<List<Cotizacion>> ListarPorClienteAsync(int clienteId) =>
        db.Cotizaciones.Include(c => c.Salon).Where(c => c.ClienteId == clienteId).OrderByDescending(c => c.CreadoUtc).ToListAsync();

    public Task<List<Cotizacion>> ListarGestionAsync() =>
        db.Cotizaciones.Include(c => c.Salon).Include(c => c.Cliente).OrderByDescending(c => c.CreadoUtc).ToListAsync();

    public Task<Cotizacion?> GetAsync(int id) => db.Cotizaciones.Include(c => c.Salon).Include(c => c.Cliente).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<OperationResult> ActualizarEstadoAsync(int id, EstadoCotizacion estado, decimal? monto = null, string? obs = null)
    {
        var c = await db.Cotizaciones.FindAsync(id);
        if (c is null) return OperationResult.Fail("Cotización no encontrada.");
        c.Estado = estado;
        if (monto.HasValue) c.MontoEstimado = monto.Value;
        if (obs is not null) c.Observaciones = obs;
        await db.SaveChangesAsync();
        return OperationResult.Success();
    }
}
