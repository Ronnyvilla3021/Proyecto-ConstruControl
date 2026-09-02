export interface Consumo {
  id: number;
  materialId: number;
  materialNombre: string;
  obraId: number;
  obraNombre: string;
  responsableId: number;
  responsableNombre: string;
  cantidad: number;
  fecha: string;
}

export interface ConsumoRequest {
  materialId: number;
  obraId: number;
  cantidad: number;
}
