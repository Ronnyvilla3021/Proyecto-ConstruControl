export type EstadoCompra = 'Pendiente' | 'Recibida' | 'Cancelada';

export interface DetalleCompra {
  materialId: number;
  materialNombre: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface Compra {
  id: number;
  proveedorId: number;
  proveedorNombre: string;
  obraId: number;
  obraNombre: string;
  fecha: string;
  fechaRecepcion: string | null;
  estado: EstadoCompra;
  total: number;
  detalles: DetalleCompra[];
}

export interface DetalleCompraRequest {
  materialId: number;
  cantidad: number;
  precioUnitario: number;
}

export interface CompraRequest {
  proveedorId: number;
  obraId: number;
  detalles: DetalleCompraRequest[];
}
