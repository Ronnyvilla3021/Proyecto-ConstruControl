import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Compra, CompraRequest } from '../models/compra.model';

@Injectable({ providedIn: 'root' })
export class CompraService {
  private base = `${environment.apiUrl}/compras`;
  constructor(private http: HttpClient) {}

  obtenerTodas(): Observable<Compra[]> {
    return this.http.get<Compra[]>(this.base);
  }
  obtenerPorId(id: number): Observable<Compra> {
    return this.http.get<Compra>(`${this.base}/${id}`);
  }
  crear(request: CompraRequest): Observable<Compra> {
    return this.http.post<Compra>(this.base, request);
  }
  recepcionar(id: number): Observable<Compra> {
    return this.http.post<Compra>(`${this.base}/${id}/recepcion`, {});
  }
}
