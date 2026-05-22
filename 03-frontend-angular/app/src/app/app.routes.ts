import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { path: 'compras', canActivate: [authGuard], loadComponent: () => import('./pages/compras/compras.component').then(m => m.ComprasComponent) },
  { path: 'ventas', canActivate: [authGuard], loadComponent: () => import('./pages/ventas/ventas.component').then(m => m.VentasComponent) },
  { path: 'kardex', canActivate: [authGuard], loadComponent: () => import('./pages/kardex/kardex.component').then(m => m.KardexComponent) },
  { path: '', redirectTo: 'kardex', pathMatch: 'full' },
  { path: '**', redirectTo: 'kardex' },
];
