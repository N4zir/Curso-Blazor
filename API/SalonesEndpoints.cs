using CumbreSalonesCongresos.Application.Services;
using CumbreSalonesCongresos.Config;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.API;

public class SalonesEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/salones");
        g.MapGet("/", async (SalonService svc, string? tipo, int? capacidadMin) =>
        {
            TipoSalon? t = Enum.TryParse<TipoSalon>(tipo, true, out var p) ? p : null;
            return Results.Ok(await svc.ListarAsync(t, capacidadMin));
        });
        g.MapGet("/{id:int}", async (int id, SalonService svc) =>
        {
            var s = await svc.GetAsync(id);
            return s is null ? Results.NotFound() : Results.Ok(s);
        });
        g.MapGet("/{id:int}/disponible", async (int id, DateTime inicio, DateTime fin, SalonService svc) =>
            Results.Ok(new { disponible = await svc.DisponibleAsync(id, inicio, fin) }));
    }
}
