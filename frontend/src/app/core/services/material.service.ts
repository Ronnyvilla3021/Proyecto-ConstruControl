import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Material, MaterialRequest } from '../models/material.model';

@Injectable({ providedIn: 'root' })
export class MaterialService {
  private base = `${environment.apiUrl}/materiales`;
  constructor(private http: HttpClient) {}

  obtenerTodos(): Observable<Material[]> {
    return this.http.get<Material[]>(this.base);
  }
  obtenerPorId(id: number): Observable<Material> {
    return this.http.get<Material>(`${this.base}/${id}`);
  }
  crear(request: MaterialRequest): Observable<Material> {
    return this.http.post<Material>(this.base, request);
  }
  actualizar(id: number, request: MaterialRequest): Observable<Material> {
    return this.http.put<Material>(`${this.base}/${id}`, request);
  }
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
