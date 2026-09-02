import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Asistencia, RegistrarEntradaRequest } from '../models/asistencia.model';

@Injectable({ providedIn: 'root' })
export class AsistenciaService {
  private base = `${environment.apiUrl}/asistencias`;
  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Asistencia[]> {
    return this.http.get<Asistencia[]>(this.base);
  }
  registrarEntrada(request: RegistrarEntradaRequest): Observable<Asistencia> {
    return this.http.post<Asistencia>(`${this.base}/entrada`, request);
  }
  registrarSalida(id: number): Observable<Asistencia> {
    return this.http.patch<Asistencia>(`${this.base}/${id}/salida`, {});
  }
}
