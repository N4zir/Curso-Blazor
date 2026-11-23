using System.Security.Claims;
using CumbreSalonesCongresos.Application.Services;
using CumbreSalonesCongresos.Config;

namespace CumbreSalonesCongresos.API;

public class ReservasEndpoints : IEndpoints
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/reservas").RequireAuthorization();

        g.MapGet("/mias", async (ClaimsPrincipal user, ClienteService clientes, ReservaService svc) =>
        {
            var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid is null) return Results.Unauthorized();
            var cliente = await clientes.GetByUserIdAsync(uid);
            return cliente is null ? Results.Ok(Array.Empty<object>()) : Results.Ok(await svc.ListarPorClienteAsync(cliente.Id));
        }).RequireAuthorization("PortalCliente");

        g.MapGet("/gestion", async (ReservaService svc) => Results.Ok(await svc.ListarGestionAsync()))
            .RequireAuthorization("PortalEmpleado");

        g.MapPost("/{id:int}/cancelar", async (int id, ClaimsPrincipal user, ClienteService clientes, ReservaService svc) =>
        {
            if (user.IsInRole("Admin") || user.IsInRole("Empleado") || user.IsInRole("Marketing"))
                return Results.Ok(await svc.CancelarAsync(id));
            var uid = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uid is null) return Results.Unauthorized();
            var cliente = await clientes.GetByUserIdAsync(uid);
            if (cliente is null) return Results.Forbid();
            return Results.Ok(await svc.CancelarAsync(id, cliente.Id));
        });
    }
}
