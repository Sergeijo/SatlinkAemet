import { APP_INITIALIZER, ApplicationConfig, PLATFORM_ID, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { isPlatformBrowser } from '@angular/common';
import { firstValueFrom } from 'rxjs';

import { OidcSecurityService, provideAuth } from 'angular-auth-oidc-client';

import { routes } from './app.routes';
import { authConfig } from './auth/auth.config';
import { satlinkAuthInterceptor } from './auth/auth.interceptor';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([satlinkAuthInterceptor])),
    provideAnimations(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          prefix: 'p',
          darkModeSelector: false,
          cssLayer: false
        }
      }
    }),
    provideClientHydration(withEventReplay()),
    provideAuth(authConfig),
    // Run checkAuth() before any route guard executes.
    // On the /auth/callback route this exchanges the authorization code for a token.
    // On all other routes it restores auth state from storage.
    // The isPlatformBrowser guard prevents this from running during SSR.
    {
      provide: APP_INITIALIZER,
      useFactory: (oidcService: OidcSecurityService, platformId: object) =>
        () => isPlatformBrowser(platformId)
          ? firstValueFrom(oidcService.checkAuth())
          : Promise.resolve(),
      deps: [OidcSecurityService, PLATFORM_ID],
      multi: true
    }
  ]
};
