using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string? Cargo { get; set; }
    public bool Activo { get; set; } = true;
}

public class ComentarioEvento
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int Calificacion { get; set; }
    public string Texto { get; set; } = string.Empty;
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
}

public class Promocion
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Alcance { get; set; }
    public decimal DescuentoPorcentaje { get; set; }
    public DateTime VigenciaInicio { get; set; }
    public DateTime VigenciaFin { get; set; }
    public bool Activa { get; set; } = true;
}

public class CasoExito
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Cliente { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Publicado { get; set; } = true;
}

public class GaleriaItem
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Categoria { get; set; }
    public int? EventoId { get; set; }
    public bool Publicado { get; set; } = true;
}

public class Participante
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Tipo { get; set; }
    public DateTime RegistradoUtc { get; set; } = DateTime.UtcNow;
}

public class Factura
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? EventoId { get; set; }
    public Evento? Evento { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}

public class Pago
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public Factura? Factura { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Metodo { get; set; } = "Transferencia";
    public string Estado { get; set; } = "Registrado";
}
