using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;

namespace CumbreSalonesCongresos.Application.Services;

public class MarketingService(ApplicationAuthDbContext db)
{
    public Task<List<Promocion>> PromocionesActivasAsync() =>
        db.Promociones.Where(p => p.Activa && p.VigenciaInicio <= DateTime.UtcNow && p.VigenciaFin >= DateTime.UtcNow).ToListAsync();
    public Task<List<CasoExito>> CasosAsync() => db.CasosExito.Where(c => c.Publicado).ToListAsync();
    public Task<List<GaleriaItem>> GaleriaAsync() => db.Galeria.Where(g => g.Publicado).ToListAsync();
    public Task<List<EntidadConvenio>> ConveniosAsync() => db.EntidadesConvenio.Where(e => e.Activa).ToListAsync();
}
