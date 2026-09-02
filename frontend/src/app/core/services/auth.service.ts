import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.model';

const TOKEN_KEY = 'construcontrol_token';
const USER_KEY = 'construcontrol_user';

interface UsuarioActual {
  nombreCompleto: string;
  email: string;
  rol: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private usuarioSignal = signal<UsuarioActual | null>(this.cargarUsuarioGuardado());

  usuario = this.usuarioSignal.asReadonly();
  estaAutenticado = computed(() => this.usuarioSignal() !== null);
  rol = computed(() => this.usuarioSignal()?.rol ?? null);

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(TOKEN_KEY, response.token);
        const usuario: UsuarioActual = {
          nombreCompleto: response.nombreCompleto,
          email: response.email,
          rol: response.rol
        };
        localStorage.setItem(USER_KEY, JSON.stringify(usuario));
        this.usuarioSignal.set(usuario);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.usuarioSignal.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private cargarUsuarioGuardado(): UsuarioActual | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
