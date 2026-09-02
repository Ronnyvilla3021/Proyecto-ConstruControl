import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Obra, ObraRequest, CambiarEstadoObraRequest } from '../models/obra.model';

@Injectable({ providedIn: 'root' })
export class ObraService {
  private base = `${environment.apiUrl}/obras`;
  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Obra[]> {
    return this.http.get<Obra[]>(this.base);
  }
  obtenerPorId(id: number): Observable<Obra> {
    return this.http.get<Obra>(`${this.base}/${id}`);
  }
  crear(request: ObraRequest): Observable<Obra> {
    return this.http.post<Obra>(this.base, request);
  }
  actualizar(id: number, request: ObraRequest): Observable<Obra> {
    return this.http.put<Obra>(`${this.base}/${id}`, request);
  }
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
  cambiarEstado(id: number, request: CambiarEstadoObraRequest): Observable<Obra> {
    return this.http.patch<Obra>(`${this.base}/${id}/estado`, request);
  }
}
