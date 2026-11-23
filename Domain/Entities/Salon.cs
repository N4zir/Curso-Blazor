using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Salon
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoSalon Tipo { get; set; }
    public int Capacidad { get; set; }
    public string? Ubicacion { get; set; }
    public string? Descripcion { get; set; }
    public string? EquipamientoBase { get; set; }
    public decimal TarifaBase { get; set; }
    public bool Activo { get; set; } = true;
    public string? ImagenUrl { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
