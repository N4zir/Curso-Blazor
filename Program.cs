using CumbreSalonesCongresos.Application.Services;
using CumbreSalonesCongresos.Components;
using CumbreSalonesCongresos.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.UseHttpClient(builder.Configuration);
builder.Services.UseAuth(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PortalCliente", p => p.RequireRole("Cliente", "ClienteVIP", "Admin"));
    options.AddPolicy("PortalEmpleado", p => p.RequireRole("Empleado", "Marketing", "Admin"));
    options.AddPolicy("PortalCocina", p => p.RequireRole("Cocina", "Admin"));
    options.AddPolicy("PortalAdmin", p => p.RequireRole("Admin"));
});

builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<SalonService>();
builder.Services.AddScoped<CotizacionService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<MarketingService>();
builder.Services.AddScoped<SeedDataService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseWhen(
    contexto => !contexto.Request.Path.StartsWithSegments("/api"),
    rama => rama.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();
app.UseAPIEndpoints();

await app.SeedAuth();
using (var scope = app.Services.CreateScope())
{
    var seed = scope.ServiceProvider.GetRequiredService<SeedDataService>();
    await seed.SeedAsync();
}

app.Run();
