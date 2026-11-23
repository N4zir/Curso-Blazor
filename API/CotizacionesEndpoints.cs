using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CumbreSalonesCongresos.Application.Services;
using CumbreSalonesCongresos.Config;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.API;

public class CotizacionesEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/cotizaciones");
        g.MapPost("/", async (Cotizacion c, CotizacionService svc) => Results.Ok(await svc.CrearSolicitudAsync(c)));

        g.MapGet("/mias", async (ClaimsPrincipal user, ClienteService clientes, CotizacionService svc) =>
        {
            var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid is null) return Results.Unauthorized();
            var cliente = await clientes.GetByUserIdAsync(uid);
            return cliente is null ? Results.Ok(Array.Empty<Cotizacion>()) : Results.Ok(await svc.ListarPorClienteAsync(cliente.Id));
        }).RequireAuthorization("PortalCliente");

        g.MapGet("/gestion", async (CotizacionService svc) => Results.Ok(await svc.ListarGestionAsync()))
            .RequireAuthorization("PortalEmpleado");

        g.MapPost("/{id:int}/estado", async (int id, EstadoRequest req, CotizacionService svc) =>
            Results.Ok(await svc.ActualizarEstadoAsync(id, req.Estado, req.Monto, req.Observaciones)))
            .RequireAuthorization("PortalEmpleado");

        g.MapPost("/{id:int}/convertir", async (int id, ReservaService svc) =>
            Results.Ok(await svc.ConvertirCotizacionAsync(id)))
            .RequireAuthorization("PortalEmpleado");
    }

    public record EstadoRequest(EstadoCotizacion Estado, decimal? Monto, string? Observaciones);
}
