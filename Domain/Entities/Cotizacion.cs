using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Cotizacion
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string? SolicitanteNombre { get; set; }
    public string? SolicitanteEmail { get; set; }
    public string? SolicitanteTelefono { get; set; }
    public string? Entidad { get; set; }
    public int? SalonId { get; set; }
    public Salon? Salon { get; set; }
    public TipoEvento TipoEvento { get; set; }
    public DateTime? FechaTentativaInicio { get; set; }
    public DateTime? FechaTentativaFin { get; set; }
    public int CantidadPersonas { get; set; }
    public string? ServiciosSolicitados { get; set; }
    public EstadoCotizacion Estado { get; set; } = EstadoCotizacion.Enviada;
    public decimal MontoEstimado { get; set; }
    public DateTime? VigenciaHasta { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
}
