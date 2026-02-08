import { Component } from '@angular/core';

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
  `]
})
export class HeaderComponent {}
