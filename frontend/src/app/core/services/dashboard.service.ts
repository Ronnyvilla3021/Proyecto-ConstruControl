import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardData } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private base = `${environment.apiUrl}/dashboard`;
  constructor(private http: HttpClient) {}

  obtenerDashboard(obraId: number): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${this.base}/${obraId}`);
  }
}
