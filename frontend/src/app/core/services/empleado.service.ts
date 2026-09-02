import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Empleado, EmpleadoRequest } from '../models/empleado.model';

@Injectable({ providedIn: 'root' })
export class EmpleadoService {
  private base = `${environment.apiUrl}/empleados`;
  constructor(private http: HttpClient) {}

  obtenerTodos(): Observable<Empleado[]> {
    return this.http.get<Empleado[]>(this.base);
  }
  crear(request: EmpleadoRequest): Observable<Empleado> {
    return this.http.post<Empleado>(this.base, request);
  }
  actualizar(id: number, request: EmpleadoRequest): Observable<Empleado> {
    return this.http.put<Empleado>(`${this.base}/${id}`, request);
  }
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
