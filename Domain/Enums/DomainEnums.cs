namespace CumbreSalonesCongresos.Domain.Enums;

public enum TipoCliente { Persona, Empresa, Institucion, Universidad, Camara, Gobierno }
public enum TipoEntidadConvenio { Empresa, Universidad, Camara, Gobierno, Institucion }
public enum TipoSalon { Ejecutivo, Auditorio, CentroConvenciones, SalaReuniones, AreaExposicion }
public enum EstadoCotizacion { Borrador, Enviada, EnRevision, Aprobada, Rechazada, Convertida, Vencida }
public enum EstadoReserva { Solicitada, Confirmada, Cancelada, Completada }
public enum EstadoEvento { Planificado, EnCurso, Finalizado, Cancelado }
public enum TipoEvento { Empresarial, Social, Academico, Congreso }
public enum EstadoContrato { Borrador, Enviado, Firmado, Anulado }
