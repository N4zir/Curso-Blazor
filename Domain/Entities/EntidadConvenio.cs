using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class EntidadConvenio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoEntidadConvenio Tipo { get; set; }
    public DateTime VigenciaInicio { get; set; } = DateTime.UtcNow.Date;
    public DateTime? VigenciaFin { get; set; }
    public bool Activa { get; set; } = true;
    public decimal PorcentajeDescuento { get; set; }
    public string? Beneficios { get; set; }
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
