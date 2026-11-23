using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Evento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TipoEvento Tipo { get; set; }
    public string? Subtipo { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? ReservaId { get; set; }
    public Reserva? Reserva { get; set; }
    public EstadoEvento Estado { get; set; } = EstadoEvento.Planificado;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Presupuesto { get; set; }
    public string? Descripcion { get; set; }
    public ICollection<CronogramaItem> Cronograma { get; set; } = new List<CronogramaItem>();
    public ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
    public ICollection<ComentarioEvento> Comentarios { get; set; } = new List<ComentarioEvento>();
}

public class CronogramaItem
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
}
