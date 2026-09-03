import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { BadgeModule } from 'primeng/badge';
import { FormsModule } from '@angular/forms';
import { AuthService } from './core/services/auth.service';
import { ThemeService } from './core/services/theme.service';

interface ItemMenu {
  label: string;
  icon: string;
  ruta: string;
  color: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, RouterLinkActive, CommonModule, FormsModule,
    ButtonModule, AvatarModule, ToggleSwitchModule, BadgeModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  sidebarAbierto = signal(false);

  menu: ItemMenu[] = [
    { label: 'Obras', icon: 'pi pi-building', ruta: '/obras', color: 'var(--cc-accent-obras)' },
    { label: 'Materiales', icon: 'pi pi-box', ruta: '/materiales', color: 'var(--cc-accent-materiales)' },
    { label: 'Compras', icon: 'pi pi-shopping-cart', ruta: '/compras', color: 'var(--cc-accent-compras)' },
    { label: 'Consumo', icon: 'pi pi-arrow-down-left', ruta: '/consumo', color: 'var(--cc-accent-alerta)' },
    { label: 'Personal', icon: 'pi pi-users', ruta: '/personal', color: 'var(--cc-accent-personal)' }
  ];

  constructor(public authService: AuthService, public themeService: ThemeService) {}

  toggleSidebar(): void {
    this.sidebarAbierto.update((v) => !v);
  }

  cerrarSidebar(): void {
    this.sidebarAbierto.set(false);
  }

  logout(): void {
    this.authService.logout();
  }
}
