import { Injectable, signal, effect } from '@angular/core';

const THEME_KEY = 'construcontrol_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  modoOscuro = signal<boolean>(this.cargarPreferencia());

  constructor() {
    effect(() => {
      const esOscuro = this.modoOscuro();
      document.documentElement.classList.toggle('app-dark', esOscuro);
      localStorage.setItem(THEME_KEY, esOscuro ? 'dark' : 'light');
    });
  }

  toggle(): void {
    this.modoOscuro.update((v) => !v);
  }

  private cargarPreferencia(): boolean {
    const guardado = localStorage.getItem(THEME_KEY);
    if (guardado) return guardado === 'dark';
    return true; // por defecto, modo oscuro (como la referencia)
  }
}
