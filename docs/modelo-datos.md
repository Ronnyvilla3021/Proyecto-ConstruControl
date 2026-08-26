# Modelo de Datos — ConstruControl

## Tablas y su propósito

| Tabla | Propósito |
|---|---|
| Usuarios | Login y control de acceso por rol |
| Obras | Proyectos de construcción gestionados |
| Materiales | Catálogo de materiales e inventario |
| Proveedores | Datos de proveedores para compras |
| Compras | Órdenes de compra (cabecera) |
| DetalleCompra | Líneas de cada compra (qué material, cuánto, a qué precio) |
| Consumos | Registro de material usado en obra |
| Empleados | Personal de obra |
| Asistencias | Registro de entrada/salida por empleado y obra |
| Facturas | Archivos adjuntos de facturas ligadas a una compra |
| FotosObra | Fotos ligadas a una obra |
| Notificaciones | Avisos generados por el motor de automatización |
| Logs | Auditoría de acciones sensibles (crear/editar/eliminar) |

## Reglas de negocio clave
1. Un material NO puede tener stock negativo. El consumo se rechaza si `Cantidad > Stock`.
2. Recepcionar una compra (`Compras.Estado = Recibida`) incrementa el stock del material automáticamente.
3. Registrar un consumo decrementa el stock del material automáticamente.
4. Los cambios de estado de una Obra siguen un flujo fijo: Planificacion -> Activa -> (Pausada <-> Activa) -> Finalizada. No se permite saltar de Planificacion a Finalizada directamente.
5. Delete es "soft delete" en todas las tablas (columna `Activo` o `Eliminado`), nunca se borra físicamente.
