import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login').then(m => m.Login) },

  {
    path: 'obras',
    canActivate: [authGuard],
    loadComponent: () => import('./features/obras/obras-list/obras-list').then(m => m.ObrasList)
  },
  {
    path: 'materiales',
    canActivate: [authGuard],
    loadComponent: () => import('./features/materiales/materiales-list/materiales-list').then(m => m.MaterialesList)
  },
  {
    path: 'compras',
    canActivate: [authGuard],
    loadComponent: () => import('./features/compras/compras-list/compras-list').then(m => m.ComprasList)
  },
  {
    path: 'consumo',
    canActivate: [authGuard],
    loadComponent: () => import('./features/consumo/consumo-list/consumo-list').then(m => m.ConsumoList)
  },
  {
    path: 'personal',
    canActivate: [authGuard],
    loadComponent: () => import('./features/personal/personal-list/personal-list').then(m => m.PersonalList)
  },
  {
    path: 'dashboard/:obraId',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard/dashboard').then(m => m.Dashboard)
  },

  { path: '', redirectTo: 'obras', pathMatch: 'full' },
  { path: '**', redirectTo: 'obras' }
];
