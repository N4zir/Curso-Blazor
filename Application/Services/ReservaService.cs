using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Application.Common;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Application.Services;

public class ReservaService(ApplicationAuthDbContext db, SalonService salonService)
{
    public async Task<OperationResult<Reserva>> CrearAsync(Reserva r)
    {
        if (r.Fin <= r.Inicio) return OperationResult<Reserva>.Fail("Rango de fechas inválido.");
        if (!await salonService.DisponibleAsync(r.SalonId, r.Inicio, r.Fin))
            return OperationResult<Reserva>.Fail("El salón no está disponible en el intervalo seleccionado.");
        r.Estado = EstadoReserva.Confirmada;
        db.Reservas.Add(r);
        await db.SaveChangesAsync();
        return OperationResult<Reserva>.Success(r);
    }

    public Task<List<Reserva>> ListarPorClienteAsync(int clienteId) =>
        db.Reservas.Include(r => r.Salon).Include(r => r.Evento).Where(r => r.ClienteId == clienteId).OrderByDescending(r => r.Inicio).ToListAsync();

    public Task<List<Reserva>> ListarGestionAsync() =>
        db.Reservas.Include(r => r.Salon).Include(r => r.Cliente).Include(r => r.Evento).OrderByDescending(r => r.Inicio).ToListAsync();

    public async Task<OperationResult> CancelarAsync(int id, int? clienteId = null)
    {
        var r = await db.Reservas.FirstOrDefaultAsync(x => x.Id == id);
        if (r is null) return OperationResult.Fail("Reserva no encontrada.");
        if (clienteId.HasValue && r.ClienteId != clienteId.Value) return OperationResult.Fail("No autorizado.");
        r.Estado = EstadoReserva.Cancelada;
        await db.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<(Reserva reserva, Evento evento, Contrato contrato)>> ConvertirCotizacionAsync(int cotizacionId)
    {
        var c = await db.Cotizaciones.Include(x => x.Cliente).FirstOrDefaultAsync(x => x.Id == cotizacionId);
        if (c is null) return OperationResult<(Reserva, Evento, Contrato)>.Fail("Cotización no encontrada.");
        if (c.ClienteId is null || c.SalonId is null || c.FechaTentativaInicio is null || c.FechaTentativaFin is null)
            return OperationResult<(Reserva, Evento, Contrato)>.Fail("La cotización debe tener cliente, salón y fechas para convertir.");
        if (c.Estado != EstadoCotizacion.Aprobada)
            return OperationResult<(Reserva, Evento, Contrato)>.Fail("La cotización debe estar aprobada.");

        await using var tx = await db.Database.BeginTransactionAsync();
        if (!await salonService.DisponibleAsync(c.SalonId.Value, c.FechaTentativaInicio.Value, c.FechaTentativaFin.Value))
            return OperationResult<(Reserva, Evento, Contrato)>.Fail("Conflicto: el salón ya no está disponible.");

        var reserva = new Reserva
        {
            SalonId = c.SalonId.Value,
            ClienteId = c.ClienteId.Value,
            CotizacionId = c.Id,
            Inicio = c.FechaTentativaInicio.Value,
            Fin = c.FechaTentativaFin.Value,
            Estado = EstadoReserva.Confirmada,
            Monto = c.MontoEstimado
        };
        db.Reservas.Add(reserva);
        await db.SaveChangesAsync();

        var evento = new Evento
        {
            Titulo = $"Evento {c.Numero}",
            Tipo = c.TipoEvento,
            ClienteId = c.ClienteId.Value,
            ReservaId = reserva.Id,
            Estado = EstadoEvento.Planificado,
            FechaInicio = c.FechaTentativaInicio.Value,
            FechaFin = c.FechaTentativaFin.Value,
            Presupuesto = c.MontoEstimado,
            Descripcion = c.Observaciones
        };
        db.Eventos.Add(evento);
        await db.SaveChangesAsync();
        reserva.EventoId = evento.Id;

        var contrato = new Contrato
        {
            Numero = $"CTR-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(100,999)}",
            CotizacionId = c.Id,
            EventoId = evento.Id,
            ClienteId = c.ClienteId.Value,
            Estado = EstadoContrato.Borrador,
            MontoTotal = c.MontoEstimado
        };
        db.Contratos.Add(contrato);
        c.Estado = EstadoCotizacion.Convertida;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return OperationResult<(Reserva, Evento, Contrato)>.Success((reserva, evento, contrato));
    }
}
