using System.Security.Claims;
using CumbreSalonesCongresos.Application.Services;
using CumbreSalonesCongresos.Config;

namespace CumbreSalonesCongresos.API;

public class EventosEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api").RequireAuthorization();

        g.MapGet("/mis-eventos", async (ClaimsPrincipal user, ClienteService clientes, EventoService svc) =>
        {
            var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid is null) return Results.Unauthorized();
            var cliente = await clientes.GetByUserIdAsync(uid);
            return cliente is null ? Results.Ok(Array.Empty<object>()) : Results.Ok(await svc.ListarPorClienteAsync(cliente.Id));
        }).RequireAuthorization("PortalCliente");

        g.MapGet("/eventos/{id:int}", async (int id, ClaimsPrincipal user, ClienteService clientes, EventoService svc) =>
        {
            if (user.IsInRole("Admin") || user.IsInRole("Empleado") || user.IsInRole("Marketing"))
                return Results.Ok(await svc.GetDetalleAsync(id));
            var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid is null) return Results.Unauthorized();
            var cliente = await clientes.GetByUserIdAsync(uid);
            if (cliente is null) return Results.Forbid();
            var ev = await svc.GetDetalleAsync(id, cliente.Id);
            return ev is null ? Results.NotFound() : Results.Ok(ev);
        });
    }
}
