import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';

import { OidcSecurityService } from 'angular-auth-oidc-client';

/**
 * Landing component for the /auth/callback route.
 * The APP_INITIALIZER already exchanged the authorization code for a token.
 * This component simply waits for authentication to be confirmed and redirects.
 */
@Component({
  selector: 'app-callback',
  standalone: true,
  template: `
    <div style="display:flex;justify-content:center;align-items:center;height:100vh;font-family:system-ui,sans-serif;color:#6b7280;">
      <p>Autenticando…</p>
    </div>
  `
})
export class CallbackComponent implements OnInit {
  private readonly oidc = inject(OidcSecurityService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.oidc.isAuthenticated$.subscribe(({ isAuthenticated }) => {
      if (isAuthenticated) {
        this.router.navigate(['/']);
      }
    });
  }
}
