using Microsoft.EntityFrameworkCore;
using CumbreSalonesCongresos.Data;
using CumbreSalonesCongresos.Domain.Entities;
using CumbreSalonesCongresos.Domain.Enums;

namespace CumbreSalonesCongresos.Application.Services;

public class SeedDataService(ApplicationAuthDbContext db)
{
    public async Task SeedAsync()
    {
        if (!await db.Salones.AnyAsync())
        {
            db.Salones.AddRange(
                new Salon { Nombre = "Salón Ejecutivo Andino", Tipo = TipoSalon.Ejecutivo, Capacidad = 40, Ubicacion = "Piso 2", Descripcion = "Ideal para reuniones y comités.", EquipamientoBase = "Proyector, audio, WiFi", TarifaBase = 350m, ImagenUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=1200&q=80" },
                new Salon { Nombre = "Auditorio Central", Tipo = TipoSalon.Auditorio, Capacidad = 300, Ubicacion = "Bloque A", Descripcion = "Escenario para conferencias y lanzamientos.", EquipamientoBase = "Sonido profesional, iluminación, pantalla", TarifaBase = 1800m, ImagenUrl = "https://images.unsplash.com/photo-1505373877841-8d25f7d46678?auto=format&fit=crop&w=1200&q=80" },
                new Salon { Nombre = "Centro de Convenciones Aurora", Tipo = TipoSalon.CentroConvenciones, Capacidad = 1200, Ubicacion = "Campus principal", Descripcion = "Gran formato para ferias y congresos.", EquipamientoBase = "LED, streaming, montaje modular", TarifaBase = 6500m, ImagenUrl = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?auto=format&fit=crop&w=1200&q=80" },
                new Salon { Nombre = "Sala de Reuniones Norte", Tipo = TipoSalon.SalaReuniones, Capacidad = 12, Ubicacion = "Piso 3", Descripcion = "Espacio privado para directorio.", EquipamientoBase = "Pantalla, videollamada", TarifaBase = 120m, ImagenUrl = "https://images.unsplash.com/photo-1431540015161-0bf868a2d407?auto=format&fit=crop&w=1200&q=80" },
                new Salon { Nombre = "Área de Exposición Prisma", Tipo = TipoSalon.AreaExposicion, Capacidad = 600, Ubicacion = "Hall principal", Descripcion = "Stands y muestras comerciales.", EquipamientoBase = "Montaje, energía, señalética", TarifaBase = 3200m, ImagenUrl = "https://images.unsplash.com/photo-1511578314322-379afb476865?auto=format&fit=crop&w=1200&q=80" }
            );
        }

        if (!await db.EntidadesConvenio.AnyAsync())
        {
            db.EntidadesConvenio.AddRange(
                new EntidadConvenio { Nombre = "Universidad Nacional", Tipo = TipoEntidadConvenio.Universidad, PorcentajeDescuento = 15, Beneficios = "Tarifas especiales para graduaciones y congresos científicos.", Activa = true },
                new EntidadConvenio { Nombre = "Cámara Empresarial Regional", Tipo = TipoEntidadConvenio.Camara, PorcentajeDescuento = 10, Beneficios = "Beneficios para foros y cumbres ejecutivas.", Activa = true },
                new EntidadConvenio { Nombre = "Alianza Corporativa Plus", Tipo = TipoEntidadConvenio.Empresa, PorcentajeDescuento = 12, Beneficios = "Descuentos por contratación anual y paquetes corporativos.", Activa = true }
            );
        }

        if (!await db.Promociones.AnyAsync())
        {
            db.Promociones.AddRange(
                new Promocion { Titulo = "Paquete Congreso Internacional", Descripcion = "Salón + producción + coffee breaks con tarifa preferencial.", Alcance = "Congresos", DescuentoPorcentaje = 10, VigenciaInicio = DateTime.UtcNow.AddDays(-30), VigenciaFin = DateTime.UtcNow.AddDays(180), Activa = true },
                new Promocion { Titulo = "Graduaciones 2026", Descripcion = "Beneficios especiales para instituciones educativas.", Alcance = "Académico", DescuentoPorcentaje = 15, VigenciaInicio = DateTime.UtcNow.AddDays(-30), VigenciaFin = DateTime.UtcNow.AddDays(240), Activa = true }
            );
        }

        if (!await db.CasosExito.AnyAsync())
        {
            db.CasosExito.AddRange(
                new CasoExito { Titulo = "Cumbre Ejecutiva Latam", Descripcion = "Más de 500 asistentes, streaming y producción integral.", Cliente = "Cámara Empresarial Regional", ImagenUrl = "https://images.unsplash.com/photo-1475721027785-f74eccf877e2?auto=format&fit=crop&w=1200&q=80", Publicado = true },
                new CasoExito { Titulo = "Gala Benéfica Anual", Descripcion = "Experiencia premium con protocolo y atención personalizada.", Cliente = "Fundación Aurora", ImagenUrl = "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?auto=format&fit=crop&w=1200&q=80", Publicado = true }
            );
        }

        if (!await db.Galeria.AnyAsync())
        {
            db.Galeria.AddRange(
                new GaleriaItem { Titulo = "Montaje corporativo", Url = "https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=1200&q=80", Categoria = "Empresarial", Publicado = true },
                new GaleriaItem { Titulo = "Congreso internacional", Url = "https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?auto=format&fit=crop&w=1200&q=80", Categoria = "Congresos", Publicado = true },
                new GaleriaItem { Titulo = "Celebración social", Url = "https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=1200&q=80", Categoria = "Social", Publicado = true }
            );
        }

        await db.SaveChangesAsync();
    }
}
