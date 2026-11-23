using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;

namespace CumbreSalonesCongresos.Application.Services;

public class EventoService(ApplicationAuthDbContext db)
{
    public Task<List<Evento>> ListarPorClienteAsync(int clienteId) =>
        db.Eventos.Include(e => e.Reserva).ThenInclude(r => r!.Salon)
            .Include(e => e.Cronograma).Include(e => e.Contratos)
            .Where(e => e.ClienteId == clienteId).OrderByDescending(e => e.FechaInicio).ToListAsync();

    public Task<Evento?> GetDetalleAsync(int id, int? clienteId = null)
    {
        var q = db.Eventos.Include(e => e.Reserva).ThenInclude(r => r!.Salon)
            .Include(e => e.Cronograma).Include(e => e.Contratos).Include(e => e.Comentarios)
            .Where(e => e.Id == id);
        if (clienteId.HasValue) q = q.Where(e => e.ClienteId == clienteId.Value);
        return q.FirstOrDefaultAsync();
    }

    public Task<List<Contrato>> ContratosPorClienteAsync(int clienteId) =>
        db.Contratos.Include(c => c.Evento).Where(c => c.ClienteId == clienteId).OrderByDescending(c => c.Fecha).ToListAsync();

    public Task<List<Participante>> ParticipantesPorClienteAsync(int clienteId) =>
        db.Participantes.Include(p => p.Evento).Where(p => p.ClienteId == clienteId).OrderByDescending(p => p.RegistradoUtc).ToListAsync();

    public async Task<Participante> AgregarParticipanteAsync(Participante p)
    { db.Participantes.Add(p); await db.SaveChangesAsync(); return p; }

    public Task<List<Factura>> FacturasPorClienteAsync(int clienteId) =>
        db.Facturas.Include(f => f.Evento).Where(f => f.ClienteId == clienteId).OrderByDescending(f => f.Fecha).ToListAsync();

    public Task<List<Pago>> PagosPorClienteAsync(int clienteId) =>
        db.Pagos.Include(p => p.Factura).Where(p => p.Factura!.ClienteId == clienteId).OrderByDescending(p => p.Fecha).ToListAsync();

    public Task<List<Evento>> HistorialClienteAsync(int clienteId) =>
        db.Eventos.Include(e => e.Reserva).ThenInclude(r => r!.Salon)
            .Where(e => e.ClienteId == clienteId && e.FechaFin < DateTime.UtcNow).OrderByDescending(e => e.FechaFin).ToListAsync();

    public async Task<ComentarioEvento> AgregarComentarioAsync(ComentarioEvento c)
    { db.Comentarios.Add(c); await db.SaveChangesAsync(); return c; }

    public Task<List<ComentarioEvento>> ComentariosPorClienteAsync(int clienteId) =>
        db.Comentarios.Include(c => c.Evento).Where(c => c.ClienteId == clienteId).OrderByDescending(c => c.CreadoUtc).ToListAsync();
}
