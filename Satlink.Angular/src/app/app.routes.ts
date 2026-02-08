import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/aemet/aemet-page.component').then(m => m.AemetPageComponent)
  }
];
