export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  nombreCompleto: string;
  email: string;
  rol: string;
}
