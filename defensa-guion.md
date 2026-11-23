# Guion de defensa — Cumbre Salones y Congresos

**Duración objetivo:** 8 minutos (rango aceptable: 5–10).
**Audiencia:** encargado del mini curso de Blazor, estudiante de un semestre superior → habla de decisiones técnicas, no de marketing.
**Eje central:** el flujo *cotización aprobada → reserva + evento + contrato en una sola transacción*. Es lo que mejor demuestra que entendiste Blazor, EF Core y el dominio.

> Regla: si te sale una pregunta larga, respondé en 30–40 segundos y volvé al eje. El banco de respuestas está en `defensa-preguntas.md`.

---

## Antes de empezar (checklist)

- [ ] `dotnet build` sin errores.
- [ ] `dotnet run` con `appsettings.Development.json`; `SeedAuth()` aplica migraciones, crea roles y el admin dev.
- [ ] Tener dos pestañas listas: una logueada como **Empleado/Admin** y otra como **Cliente** (o usar la misma sesión Admin, que pertenece a todas las políticas).
- [ ] Tener una cotización en estado `Aprobada` precargada para la demo (si no, el botón "Convertir" no aparece/habilita).
- [ ] Capturas de pantalla de respaldo en caso de que falle el WiFi o la demo.
- [ ] Saber de memoria la ruta: `/` → `/salones` → `/cotizar` → `/empleado/cotizaciones` → `/cliente/reservas`.

---

## Bloque 1 — Apertura y problema (0:00–0:45)

**Decir:**

> "Buenos días. Les presento **Cumbre Salones y Congresos**, una plataforma web para gestionar el ciclo completo de un evento: desde que un cliente ve el catálogo de salones y pide una cotización, hasta que esa cotización se convierte en reserva, evento y contrato. El problema que resuelve es que esos pasos normalmente viven en planillas, mails y sistemas separados; acá están en un solo flujo con un solo modelo de datos."

**Mostrar:** slide 1 y 2.
**Transición:** "Les muestro cómo está armado y después el flujo que considero el corazón del proyecto."

---

## Bloque 2 — Stack y arquitectura (0:45–2:30)

**Decir:**

> "El stack es **Blazor Web App con .NET 10** en modo *Interactive Server*, **Minimal APIs**, **EF Core 10** con **ASP.NET Core Identity** y **Tailwind** para la UI. La decisión principal fue usar C# de punta a punta: los componentes Razor y los servicios comparten entidades y reglas, sin mantener un front en JavaScript aparte. El costo es que la interacción depende del circuito SignalR con el servidor; para este dominio, donde las operaciones son de negocio y no animaciones pesadas, me pareció un buen trade-off."

> "El proyecto está separado en capas: **`Config/`** concentra el arranque —`UseAuth()`, `UseDB()`, registro automático de endpoints—; **`Domain/`** tiene entidades y enums; **`Application/`** los servicios y un `OperationResult` uniforme; **`Data/`** el DbContext; **`API/`** los endpoints agrupados por módulo mediante la interfaz `IEndpoints`; y **`Components/`** las páginas Razor de los tres portales."

**Mostrar:** slide 3 y 4 (diagrama de capas).
**Transición:** "Antes del flujo, dos palabras sobre el modelo de datos."

---

## Bloque 3 — Modelo de datos (2:30–3:30)

**Decir:**

> "Hay **un solo `ApplicationAuthDbContext`** que hereda de `IdentityDbContext<ApplicationUser>`: contiene las tablas de Identity y las 16 entidades del dominio. Lo elegí así para poder hacer transacciones entre un usuario, un cliente, una reserva y un contrato sin coordinar dos contextos. En `OnModelCreating` puse precisión decimal `18,2` para montos, índices únicos en números de cotización y contrato, un índice compuesto en `Reserva(SalonId, Inicio, Fin, Estado)` para las consultas de disponibilidad, y reglas de borrado explícitas —por ejemplo `SetNull` entre Reserva y Evento y `Restrict` en Contrato."

**Mostrar:** slide 5. No leer la lista de entidades; marcar solo Cliente, Cotización, Reserva, Evento y Contrato.
**Transición:** "Con ese modelo, el flujo crítico."

---

## Bloque 4 — El flujo estrella (3:30–5:00) ⭐

**Decir:**

> "Un visitante entra a `/cotizar` y manda una solicitud: `CotizacionService.CrearSolicitudAsync` valida fechas, genera un número `COT-...`, la guarda en estado `Enviada` y, si el usuario está autenticado, la vincula a su `Cliente`."

> "Un **Empleado** la revisa en `/empleado/cotizaciones`, cambia el estado a `Aprobada` y carga el monto estimado. Cuando aprieta **Convertir**, `ReservaService.ConvertirCotizacionAsync` hace tres cosas: valida que la cotización esté aprobada y completa, **revisa disponibilidad del salón dentro de la transacción**, y crea la `Reserva`, el `Evento` y el `Contrato`, cambiando la cotización a `Convertida`. Todo con `BeginTransactionAsync` / `CommitAsync`: si falla el contrato, no queda una reserva huérfana."

> "La disponibilidad es `SalonService.DisponibleAsync`: bloquean los estados `Solicitada` y `Confirmada`, y se detecta solapamiento con `inicio < r.Fin && fin > r.Inicio`, que es la comparación correcta para intervalos semiabiertos."

**Mostrar:** slide 6 (diagrama). Si podés, abrir `ReservaService.cs` líneas 38–93 durante 10 segundos.
**Frase clave para impresionar:** "La revalidación va dentro de la transacción justamente porque entre la aprobación y la conversión puede pasar tiempo y otro cliente pudo haber reservado el salón."
**Transición:** "Todo esto está detrás de tres portales con autorización."

---

## Bloque 5 — Seguridad (5:00–5:45)

**Decir:**

> "La autenticación es **ASP.NET Core Identity** con cookie HttpOnly y `RequireConfirmedAccount`. Los roles se siembran desde configuración: Admin, Empleado, Marketing, Cocina, Cliente y ClienteVIP. En `Program.cs` definí cuatro políticas —`PortalCliente`, `PortalEmpleado`, `PortalCocina` y `PortalAdmin`— que se aplican con `[Authorize(Policy = ...)]` en las páginas y con `RequireAuthorization` en los endpoints. Las rutas `/api` responden **401/403** en vez de redirigir al login, y `UseAntiforgery()` protege los formularios. `SeedAuth()` es idempotente: aplica migraciones, crea roles y el admin, y se puede correr en cada arranque."

**Mostrar:** slide 7.
**Transición:** "Y con eso, les hago una demo rápida de punta a punta."

---

## Bloque 6 — Demo en vivo (5:45–7:00)

**Guion de clicks (no improvises):**

1. `/` → mostrás la home pública.
2. `/salones` → filtrás por tipo/capacidad (si hay datos cargados por `SeedDataService`).
3. `/cotizar` → completás el formulario y enviás → decís: *"queda en estado Enviada"*.
4. Loguearte como **admin/empleado** → `/empleado/cotizaciones` → aprobás una cotización con monto.
5. Botón **Convertir a reserva + evento + contrato** → decís: *"ahora están creados los tres registros en una transacción"*.
6. `/cliente/reservas` → mostrás la reserva ya visible para el cliente.

**Si algo falla:** no te quedés callado. Decí *"se ve el principio en la captura"* y pasás a la slide 8. Tené las capturas abiertas en otra pestaña.

**Transición:** "Cierro con un balance honesto."

---

## Bloque 7 — Cierre y mejoras (7:00–8:00)

**Decir:**

> "Como alcance actual están implementadas las fases 0 a 3: dominio y datos, sitio público y captación, y los portales de cliente y empleado con el flujo comercial mínimo. Lo que **no** está, y lo diría en una entrevista sin dudarlo: no hay tests automatizados, el correo es `IdentityNoOpEmailSender`, los pagos y el storage son pendientes, y la validación de disponibilidad tiene una ventana de carrera si dos conversiones llegan exactamente al mismo tiempo. Para producción yo agregaría bloqueo pesimista o un aislamiento `SERIALIZABLE` / exclusión por rango en PostgreSQL, más tests de integración sobre la conversión, y mover las migraciones del arranque a un paso del deploy si hubiera varias réplicas."

> "Eso es todo. Gracias."

**Mostrar:** slide 8.

---

## Reglas finales

- Si te preguntan algo que no sabés: **"No lo implementé así, pero lo resolvería..."** y proposé una alternativa. Nunca inventes código que no existe.
- Si te piden ver código, abrí en este orden: `Program.cs` → `ReservaService.cs` → `SalonService.cs` → `ApplicationDbContext.cs` → `API/CotizacionesEndpoints.cs`.
- No te extendas más de 40 segundos en una respuesta; el evaluador va a querer seguir con la demo.
- El README (`README.md`) es tu red de seguridad: está casi todo resumido ahí.
