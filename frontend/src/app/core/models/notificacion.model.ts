export type TipoNotificacion = 'StockBajo' | 'PresupuestoExcedido' | 'CompraSugerida';

export interface Notificacion {
  id: number;
  tipo: TipoNotificacion;
  obraId: number | null;
  obraNombre: string | null;
  materialId: number | null;
  materialNombre: string | null;
  mensaje: string;
  leida: boolean;
  fechaCreacion: string;
}
