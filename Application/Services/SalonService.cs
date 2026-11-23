using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Application.Services;

public class SalonService(ApplicationAuthDbContext db)
{
    public async Task<List<Salon>> ListarAsync(TipoSalon? tipo = null, int? capacidadMin = null)
    {
        var q = db.Salones.Where(s => s.Activo).AsQueryable();
        if (tipo.HasValue) q = q.Where(s => s.Tipo == tipo);
        if (capacidadMin.HasValue) q = q.Where(s => s.Capacidad >= capacidadMin.Value);
        return await q.OrderBy(s => s.Nombre).ToListAsync();
    }

    public Task<Salon?> GetAsync(int id) => db.Salones.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<bool> DisponibleAsync(int salonId, DateTime inicio, DateTime fin, int? excluirReservaId = null)
    {
        if (fin <= inicio) return false;
        var bloqueantes = new[] { EstadoReserva.Solicitada, EstadoReserva.Confirmada };
        return !await db.Reservas.AnyAsync(r => r.SalonId == salonId
            && (excluirReservaId == null || r.Id != excluirReservaId)
            && bloqueantes.Contains(r.Estado)
            && inicio < r.Fin && fin > r.Inicio);
    }
}
