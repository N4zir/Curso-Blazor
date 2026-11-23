using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public Salon? Salon { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int? CotizacionId { get; set; }
    public Cotizacion? Cotizacion { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public EstadoReserva Estado { get; set; } = EstadoReserva.Solicitada;
    public decimal Monto { get; set; }
    public string? Notas { get; set; }
}
