export interface Proveedor {
  id: number;
  nombre: string;
  contacto?: string;
  telefono?: string;
  email?: string;
}

export interface ProveedorRequest {
  nombre: string;
  contacto?: string;
  telefono?: string;
  email?: string;
}
