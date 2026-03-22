import { Routes } from '@angular/router';

import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./features/aemet/aemet-page.component').then(m => m.AemetPageComponent)
  },
  {
    // Duende IS redirects here after login with ?code=...&state=...
    path: 'auth/callback',
    loadComponent: () => import('./auth/callback.component').then(m => m.CallbackComponent)
  },
  { path: '**', redirectTo: '' }
];
