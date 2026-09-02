export interface MaterialCritico {
  materialId: number;
  nombre: string;
  stock: number;
  stockMinimo: number;
}

export interface DashboardData {
  obraId: number;
  obraNombre: string;
  presupuesto: number;
  gastoTotal: number;
  porcentajePresupuestoUsado: number;
  diasTranscurridos: number;
  costoDiarioPromedio: number;
  materialesCriticos: MaterialCritico[];
}
