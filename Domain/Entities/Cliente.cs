using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public TipoCliente Tipo { get; set; } = TipoCliente.Persona;
    public string NombreRazonSocial { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? EmailContacto { get; set; }
    public string? Telefono { get; set; }
    public int? EntidadConvenioId { get; set; }
    public EntidadConvenio? EntidadConvenio { get; set; }
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
