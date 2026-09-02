import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Notificacion } from '../models/notificacion.model';

@Injectable({ providedIn: 'root' })
export class NotificacionService {
  private base = `${environment.apiUrl}/notificaciones`;
  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Notificacion[]> {
    return this.http.get<Notificacion[]>(this.base);
  }
  marcarLeida(id: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/leida`, {});
  }
}
