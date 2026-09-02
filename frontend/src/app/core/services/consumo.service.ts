import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Consumo, ConsumoRequest } from '../models/consumo.model';

@Injectable({ providedIn: 'root' })
export class ConsumoService {
  private base = `${environment.apiUrl}/consumo`;
  constructor(private http: HttpClient) {}

  obtenerTodos(): Observable<Consumo[]> {
    return this.http.get<Consumo[]>(this.base);
  }
  registrar(request: ConsumoRequest): Observable<Consumo> {
    return this.http.post<Consumo>(this.base, request);
  }
}
