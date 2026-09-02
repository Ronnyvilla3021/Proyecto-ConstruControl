export type EstadoObra = 'Planificacion' | 'Activa' | 'Pausada' | 'Finalizada';

export interface Obra {
  id: number;
  nombre: string;
  ubicacion: string;
  presupuesto: number;
  fechaInicio: string;
  estado: EstadoObra;
}

export interface ObraRequest {
  nombre: string;
  ubicacion: string;
  presupuesto: number;
  fechaInicio: string;
}

export interface CambiarEstadoObraRequest {
  nuevoEstado: EstadoObra;
}
