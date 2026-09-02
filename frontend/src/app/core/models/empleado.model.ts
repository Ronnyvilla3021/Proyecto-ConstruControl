export interface Empleado {
  id: number;
  nombre: string;
  cargo: string;
  fechaIngreso: string;
}

export interface EmpleadoRequest {
  nombre: string;
  cargo: string;
  fechaIngreso: string;
}
