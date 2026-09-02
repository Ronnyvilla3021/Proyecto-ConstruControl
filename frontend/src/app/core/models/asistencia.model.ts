export interface Asistencia {
  id: number;
  empleadoId: number;
  empleadoNombre: string;
  obraId: number;
  obraNombre: string;
  fecha: string;
  horaEntrada: string | null;
  horaSalida: string | null;
}

export interface RegistrarEntradaRequest {
  empleadoId: number;
  obraId: number;
}
