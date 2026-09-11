import { Injectable, signal, effect, inject } from '@angular/core';
import { AuthService } from './auth.service';

const THEME_KEY = 'construcontrol_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private authService = inject(AuthService);
  modoOscuro = signal<boolean>(this.cargarPreferencia());

  constructor() {
    effect(() => {
      const activarOscuro = this.authService.estaAutenticado() && this.modoOscuro();
      document.documentElement.classList.toggle('app-dark', activarOscuro);
      localStorage.setItem(THEME_KEY, this.modoOscuro() ? 'dark' : 'light');
    });
  }

  toggle(): void {
    this.modoOscuro.update((v) => !v);
  }

  private cargarPreferencia(): boolean {
    const guardado = localStorage.getItem(THEME_KEY);
    if (guardado) return guardado === 'dark';
    return true;
  }
}
