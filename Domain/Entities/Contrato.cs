using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Contrato
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int? CotizacionId { get; set; }
    public Cotizacion? Cotizacion { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public EstadoContrato Estado { get; set; } = EstadoContrato.Borrador;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal MontoTotal { get; set; }
    public string? DocumentoUrl { get; set; }
}
