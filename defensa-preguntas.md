# Banco de preguntas — defensa de Cumbre Salones y Congresos

12 preguntas probables con respuestas concretas. Formato: **respuesta corta** (para decir en voz alta) y **dónde está / profundidad** (por si el evaluador quiere ver código).

> Comodín cuando no sepas algo: *"No lo implementé así, pero lo resolvería…"* + alternativa. Nunca inventes una clase que no existe.

---

## 1. ¿Por qué Blazor Server y no React/Angular?

**Corta:** "Para no mantener dos lenguajes ni una capa API duplicada: los componentes y los servicios comparten las mismas entidades y reglas en C#. Interactive Server usa SignalR para los eventos de UI. El trade-off es que la latencia depende del servidor y hay un circuito por usuario; para este sistema, donde casi todo son formularios y consultas, me conviene."

**Profundidad:** `Program.cs:26-27, 51-52` → `AddInteractiveServerComponents()` / `AddInteractiveServerRenderMode()`. No hay proyecto frontend separado en el repo.

---

## 2. ¿Por qué un solo DbContext para Identity y el dominio?

**Corta:** "Para poder hacer transacciones entre un `ApplicationUser`, su `Cliente`, una reserva y un contrato sin coordinar dos contextos ni dos bases. El costo es acoplamiento: el dominio vive en el mismo contexto que Identity."

**Profundidad:** `Data/ApplicationDbContext.cs:7` → `IdentityDbContext<ApplicationUser>` con los `DbSet` del dominio. `Config/CumbreDB.cs:28-43` decide qué contexto registrar según `conAuth`; no se registran ambos a la vez.

---

## 3. ¿Cómo evitás que se reserve dos veces el mismo salón?

**Corta:** "Con `SalonService.DisponibleAsync`: revisa que no exista una reserva del salón en ese rango con estado `Solicitada` o `Confirmada`, usando `inicio < r.Fin && fin > r.Inicio`. Y la clave: en la conversión de cotización **vuelvo a chequear dentro de la transacción**, porque entre aprobar y convertir puede pasar tiempo."

**Profundidad (y acá sé honesto):** `Application/Services/SalonService.cs:20-27` y `ReservaService.cs:47-49`.
**Limitación real:** es un check-then-act; dos conversiones concurrentes pueden pasar el `AnyAsync` al mismo tiempo. No hay lock ni `SERIALIZABLE`.
**Solución que propondría:** `SERIALIZABLE`/`SELECT ... FOR UPDATE` sobre el salón, o en PostgreSQL una constraint de exclusión con `tstzrange` + `giftexclude`. Para SQLite, serializar escrituras con un semáforo o locks de EF.

---

## 4. ¿Qué pasa si falla el INSERT del contrato?

**Corta:** "Se hace rollback de los tres inserts: la reserva, el evento y el contrato entran en `BeginTransactionAsync` y solo se confirman con `CommitAsync`. El método devuelve un `OperationResult` con `Fail`, no lanza excepción hacia la UI."

**Profundidad:** `ReservaService.cs:38-93`. Ojo con la precisión: el rollback ocurre si hay **excepción** (el `await using` descarta la transacción); si un `SaveChanges` devuelve 0 filas sin excepciones, el código no lo detecta. Mejoría: `CommitAsync` dentro de try/catch y devolver `Fail` con el mensaje.

---

## 5. ¿Por qué Minimal APIs en vez de Controllers?

**Corta:** "Porque los endpoints son pequeños y están agrupados por módulo. Definí la interfaz `IEndpoints` y `UseAPIEndpoints()` los descubre por reflexión al arrancar, así que agregar un módulo no implica tocar `Program.cs`."

**Profundidad:** `Config/CumbreAPIEndpoints.cs:11-30` y `API/*.cs`. Ejemplo: `CotizacionesEndpoints.cs` mapea `/api/cotizaciones`, `/mias`, `/gestion`, `/{id}/estado` y `/{id}/convertir`.

---

## 6. ¿Cómo se siembra la base?

**Corta:** "Dos pasos en el arranque: `SeedAuth()` aplica migraciones, crea los roles de `Auth:Roles` y el admin si hay credenciales configuradas; `SeedDataService.SeedAsync()` carga salones, convenios, promociones y galería. `SeedAuth` es idempotente: si el rol o el usuario ya existen, no los duplica."

**Profundidad:** `Program.cs:57-62`, `Config/CumbreAuth.cs:89-154`. Detalle que suma: el admin se crea con `EmailConfirmed = true` porque `RequireConfirmedAccount` está activo y el email sender es no-op en dev.
**Mejoría:** migrar al arrancar está bien para una instancia; con varias réplicas conviene aplicar migraciones en un paso separado del deploy.

---

## 7. ¿Cómo protegés los portales?

**Corta:** "Con cuatro políticas de autorización en `Program.cs`: `PortalCliente`, `PortalEmpleado`, `PortalCocina` y `PortalAdmin`, basadas en roles. Se aplican de dos formas: `[Authorize(Policy = "...")]` en cada página Razor y `RequireAuthorization("...")` en los grupos de endpoints. Si no está autenticado, `RedirectToLogin`; si está autenticado pero sin rol, mensaje de 'sin permisos'."

**Profundidad:** `Program.cs:10-16`, `Components/Routes.razor:7-18`, `Components/Pages/Cliente/*.razor` y `Empleado/*.razor` (todos con el atributo). Las políticas incluyen a `Admin` en las tres primeras.

---

## 8. ¿Cómo se vincula un usuario de Identity con un Cliente?

**Corta:** "La tabla `Cliente` tiene `UserId` apuntando al `ApplicationUser`. `ClienteService.EnsureClienteAsync` verifica si ya existe; si no, lo crea con el nombre y email del usuario y guarda el `ClienteId` en el usuario. Es la misma idea que un perfil lazy: se arma al primer acceso."

**Profundidad:** `Application/Services/ClienteService.cs:10-39` (`EnsureClienteAsync`, `GetByUserIdAsync`, `GetOrCreateByUserIdAsync`). Hay índice en `Cliente.UserId` (`ApplicationDbContext.cs:29`) e índice **único** en `Empleado.UserId` (línea 30).

---

## 9. ¿Por qué configuraste tres motores de base de datos?

**Corta:** "Para que el mismo código corra en local con SQLite y en producción con PostgreSQL o SQL Server. `UseDB` lee la sección `Database` de configuración, arma la cadena con el builder de cada driver y aplica el proveedor de EF Core correspondiente. Las variables de entorno con `__` pisan la configuración, así que no hay secretos en el repo."

**Profundidad:** `Config/CumbreDB.cs:34-48, 107-125, 131-160`. Falla rápido si falta una clave (`Requerido`), con el nombre de la variable a usar. `Database__Engine=POSTGRESQL` en `.github`/deploy, sin tocar código.

---

## 10. ¿Qué es `OperationResult` y por qué no lanzás excepciones?

**Corta:** "Es un tipo resultado con `Success`/`Fail` y un mensaje. Lo uso en los servicios para que las reglas de negocio —fechas inválidas, salón ocupado, cotización no aprobada— lleguen a la UI como un mensaje controlado en vez de una excepción. Las excepciones las dejo para fallos técnicos."

**Profundidad:** `Application/Common/OperationResult.cs`. Ejemplos: `CotizacionService.CrearSolicitudAsync:13-14`, `ReservaService.CrearAsync:13-15, 31-32`.

---

## 11. ¿Hay tests? ¿Qué testearías?

**Corta (sin excusas):** "No hay tests automatizados todavía; es una de las deudas que señalaría. Si los agrego, priorizo tres cosas: el cálculo de solapamiento de `DisponibleAsync` con casos borde, el rollback de `ConvertirCotizacionAsync` cuando falla el contrato, y que los endpoints con `RequireAuthorization` devuelvan 401/403 para el rol equivocado."

**Profundidad:** No existe proyecto de tests en el repo. Path natural: `tests/CumbreSalonesCongresos.Tests` con xUnit + `WebApplicationFactory` para integración. Para disponibilidad conviene un SQLite en memoria por test.

---

## 12. ¿Qué le falta para producción?

**Corta:** "Cuatro cosas: concurrencia robusta en la disponibilidad, correo real —hoy es `IdentityNoOpEmailSender`—, pasarela de pagos y storage, y auditoría. Además, migraciones fuera del arranque y secrets por entorno. Funcionalmente el flujo comercial mínimo ya está completo."

**Profundidad:** `README.md` → sección *Notas* (línea 107 en adelante). `Config/CumbreAuth.cs:60` → `IdentityNoOpEmailSender`. `Config/CumbreAuth.cs:85-87` → observación sobre migraciones y réplicas. `Components/Pages/Cliente/Pagos.razor` y `Facturas.razor` son pantallas de consulta, no cobro real.

---

## Preguntas bonus (si sobra tiempo)

- **¿Por qué `Remove`/`SetNull` en las relaciones?** Para no borrar un evento al cancelar una reserva: `Reserva.Evento` usa `DeleteBehavior.SetNull`; `Contrato` usa `Restrict` para no perder documentos; `ComentarioEvento` sí es `Cascade` porque no tiene sentido huérfano (`ApplicationDbContext.cs:42-58`).
- **¿Cómo funciona el `HttpClient` scoped?** `Config/CumbreHttpClient.cs` registra un `HttpClient` con `CookieContainer` para que las llamadas internas a la API lleven la sesión del usuario.
- **¿Qué son `CookieState` y `RedirigirOResponder`?** El primero permite pasar cookies al render (para prerender/Blazor); el segundo hace que `/api` devuelva 401/403 y el resto redirija al login (`CumbreAuth.cs:156-164`).
- **¿Por qué los números de cotización y contrato llevan timestamp + random?** Para ser legibles y únicos sin depender de una secuencia de base; conviven con índices únicos por si colisionan.
