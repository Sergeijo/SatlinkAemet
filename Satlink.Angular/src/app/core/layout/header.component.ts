import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';

import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-header',
  standalone: true,
  template: `
    <header class="app-header">
      <div class="header-content">
        <div class="logo-section">
          <i class="pi pi-cloud"></i>
          <span class="app-title">Satlink AEMET</span>
        </div>
        <div class="subtitle">Predicciones Marítimas</div>
        @if (isAuthenticated()) {
          <div class="user-section">
            <span class="username">{{ userName() }}</span>
            <button class="logout-btn" (click)="logout()">Cerrar sesión</button>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .app-header {
      background: linear-gradient(135deg, #1e40af 0%, #3b82f6 100%);
      color: white;
      padding: 24px;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
      margin-bottom: 32px;
    }

    .header-content {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      align-items: center;
      justify-content: space-between;
      flex-wrap: wrap;
      gap: 12px;
    }

    .logo-section {
      display: flex;
      align-items: center;
      gap: 14px;
    }

    .logo-section i {
      font-size: 38px;
      opacity: 0.95;
    }

    .app-title {
      font-size: 28px;
      font-weight: 700;
      letter-spacing: -0.5px;
    }

    .subtitle {
      font-size: 15px;
      opacity: 0.9;
      font-weight: 500;
    }

    .user-section {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .username {
      font-size: 0.9rem;
      opacity: 0.9;
    }

    .logout-btn {
      background: rgba(255, 255, 255, 0.15);
      border: 1px solid rgba(255, 255, 255, 0.4);
      color: white;
      padding: 6px 16px;
      border-radius: 6px;
      font-size: 0.85rem;
      cursor: pointer;
      transition: background 0.15s;
    }

    .logout-btn:hover {
      background: rgba(255, 255, 255, 0.28);
    }
  `]
})
export class HeaderComponent {
  private readonly oidcService = inject(OidcSecurityService);

  readonly isAuthenticated = toSignal(
    this.oidcService.isAuthenticated$.pipe(map(({ isAuthenticated }) => isAuthenticated)),
    { initialValue: false }
  );

  readonly userName = toSignal(
    this.oidcService.userData$.pipe(
      map(({ userData }) => userData?.name ?? userData?.preferred_username ?? userData?.email ?? '')
    ),
    { initialValue: '' }
  );

  logout(): void {
    this.oidcService.logoff().subscribe();
  }
}

