export interface Material {
  id: number;
  nombre: string;
  stock: number;
  stockMinimo: number;
  unidad: string;
  precioUnitario: number;
  stockBajo: boolean;
}

export interface MaterialRequest {
  nombre: string;
  stockMinimo: number;
  unidad: string;
  precioUnitario: number;
}
