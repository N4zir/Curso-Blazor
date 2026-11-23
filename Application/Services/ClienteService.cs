using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Application.Services;

public class ClienteService(ApplicationAuthDbContext db)
{
    public async Task<Cliente> EnsureClienteAsync(ApplicationUser user, string? nombre = null, string? email = null)
    {
        var existing = await db.Clientes.FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (existing is not null) return existing;

        var cliente = new Cliente
        {
            UserId = user.Id,
            Tipo = TipoCliente.Persona,
            NombreRazonSocial = nombre ?? user.NombreCompleto ?? user.Email ?? "Cliente",
            EmailContacto = email ?? user.Email,
            Telefono = user.PhoneNumber
        };
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        user.ClienteId = cliente.Id;
        db.Users.Update(user);
        await db.SaveChangesAsync();
        return cliente;
    }

    public Task<Cliente?> GetByUserIdAsync(string userId) => db.Clientes.Include(c => c.EntidadConvenio).FirstOrDefaultAsync(c => c.UserId == userId);
    public async Task<Cliente?> GetOrCreateByUserIdAsync(string userId)
    {
        var c = await GetByUserIdAsync(userId);
        if (c is not null) return c;
        var user = await db.Users.FindAsync(userId);
        if (user is null) return null;
        return await EnsureClienteAsync(user);
    }
    public Task<Cliente?> GetByIdAsync(int id) => db.Clientes.Include(c => c.EntidadConvenio).FirstOrDefaultAsync(c => c.Id == id);
    public async Task UpdatePerfilAsync(Cliente cliente)
    {
        db.Clientes.Update(cliente);
        await db.SaveChangesAsync();
    }
}
