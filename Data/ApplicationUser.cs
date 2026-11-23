using Microsoft.AspNetCore.Identity;

namespace CumbreSalonesCongresos.Data;

public class ApplicationUser : IdentityUser
{
    public string? NombreCompleto { get; set; }
    public bool EsVip { get; set; }
    public bool Activo { get; set; } = true;
    public int? ClienteId { get; set; }
    public int? EmpleadoId { get; set; }
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
}

