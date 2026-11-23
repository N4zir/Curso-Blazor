# Cumbre Salones y Congresos

Plataforma web para gestión integral de eventos, salones y congresos, construida sobre **Blazor Web App (.NET 10)**, **Tailwind CSS**, **EF Core** y **ASP.NET Core Identity**. Parte de una plantilla base con arranque encapsulado en `Config/` y la extiende con dominio, portales y APIs para operación comercial y de eventos.

## Stack

| Capa | Tecnología |
|---|---|
| Frontend | Blazor Web App (.NET 10), render interactivo servidor, Tailwind CSS |
| Backend | ASP.NET Core 10, Minimal APIs por contrato `IEndpoints` |
| Datos | EF Core 10 + `ApplicationAuthDbContext` (Identity + dominio) |
| Auth | ASP.NET Core Identity, cookies HttpOnly, roles/políticas |
| DB | SQLite (dev por defecto), PostgreSQL, SQL Server |

## Requisitos

| | |
|---|---|
| .NET SDK | 10.0 |
| Node + pnpm | para Tailwind (el build lo invoca) |
| `dotnet-ef` | en `dotnet-tools.json` → `dotnet tool restore` |

```bash
dotnet tool restore
dotnet build
dotnet tool run dotnet-ef database update --context ApplicationAuthDbContext
dotnet run
```

La primera compilación ejecuta `pnpm install` y genera `wwwroot/app.css`. En dev, `SeedAuth()` aplica migraciones/roles/admin y `SeedDataService` carga datos de referencia (salones, convenios, promociones, galería).

## Configuración

Configuración en `appsettings.json`, sobreescribible por variables de entorno con `__`:

```bash
Database__Engine=POSTGRESQL
Database__Password=...
Auth__AdminPassword=...
Api__BaseUrl=https://mi-dominio.com/
```

> No subas contraseñas al repo. Usa variables de entorno o `dotnet user-secrets`.

### Base de datos (`UseDB`)

```json
"Database": { "Engine": "SQLITE", "Source": "Data/app.db" }
```

| `Engine` | Campos |
|---|---|
| `SQLITE` | `Source` |
| `POSTGRESQL` | `Host`, `Port`, `Name`, `User`, `Password` |
| `SQLSERVER` | ídem + `TrustServerCertificate` |

### Auth y roles (`UseAuth`)

`UseAuth` configura Identity y llama a `UseDB` internamente (no llamar ambos). Registra `ApplicationAuthDbContext` como contexto único de Identity + dominio.

Roles sembrados por configuración (`Auth:Roles`): `Admin, Empleado, Marketing, Cocina, Cliente, ClienteVIP`. El admin dev se configura en `appsettings.Development.json` (no usar esa contraseña en producción).

Políticas registradas en `Program.cs`: `PortalCliente`, `PortalEmpleado`, `PortalCocina`, `PortalAdmin`.

## Módulos implementados (Fases 0–3)

### Fase 0 — Base
- Limpieza del dominio de ejemplo y estructura por capas: `Domain/`, `Application/`, `Infrastructure/Persistence/`, `Services/`, `API/`, `Components/`.
- DI de servicios, políticas de autorización, seed de dominio.

### Fase 1 — Dominio y datos
- Entidades núcleo: `Cliente`, `EntidadConvenio`, `Salon`, `Cotizacion`, `Reserva`, `Evento`, `CronogramaItem`, `Contrato`, `Empleado`, `ComentarioEvento`, `Promocion`, `CasoExito`, `GaleriaItem`, `Participante`, `Factura`, `Pago`.
- `ApplicationAuthDbContext` con precisión decimal, índices y relaciones.
- Servicios: `ClienteService`, `SalonService` (disponibilidad), `CotizacionService`, `ReservaService` (validación de solapamiento + conversión transaccional cotización→reserva+evento+contrato), `EventoService`, `MarketingService`, `SeedDataService`.

### Fase 2 — Sitio público y captación
- Rutas públicas: `/`, `/nosotros`, `/eventos`, `/congresos`, `/servicios`, `/catering`, `/galeria`, `/convenios`, `/contacto`, `/salones`, `/cotizar`.
- Catálogo de salones con filtros y CTA a cotización.
- Solicitud de cotización persistida; vincula cliente autenticado cuando aplica.

### Fase 3 — Portal cliente y flujo comercial mínimo
- Portal cliente `/cliente/*` (policy `PortalCliente`): dashboard, cotizaciones, reservas, eventos, cronogramas, contratos, facturas, pagos, participantes, historial, comentarios, perfil.
- Alta/vinculación de `Cliente` al registrar o primer acceso.
- Portal empleado mínimo `/empleado/*` (policy `PortalEmpleado`): dashboard, bandeja de cotizaciones, cambio de estado, monto estimado, conversión a reserva+evento+contrato, listado de reservas.
- APIs `IEndpoints`: `SalonesEndpoints`, `CotizacionesEndpoints`, `EventosEndpoints`, `ReservasEndpoints`.

## Estructura

```text
Config/            CumbreDB, CumbreAuth, CumbreHttpClient, CumbreAPIEndpoints
API/               Endpoints por módulo (IEndpoints)
Domain/            Entidades y enums
Application/       Servicios de aplicación y resultados
Data/              ApplicationAuthDbContext, ApplicationUser, seed/migraciones
Components/        Layouts, páginas públicas, cliente, empleado, Account
Styles/            Tailwind input
wwwroot/           app.css generado
```

## Migraciones

```bash
dotnet tool run dotnet-ef migrations add <Nombre> --context ApplicationAuthDbContext
dotnet tool run dotnet-ef database update --context ApplicationAuthDbContext
```

## Notas

- `*.db` se ignora en git; la DB local se genera por migración/seed.
- `RequireConfirmedAccount` está activo; el emisor de correo por defecto es no-op en dev.
- Para producción: configurar secretos por entorno, correo real, storage, pasarela de pagos y endurecer auditoría/concurrencia según despliegue.
