import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';

import { OidcSecurityService } from 'angular-auth-oidc-client';
import { map, take } from 'rxjs/operators';

/**
 * Redirects unauthenticated users to Duende Identity Server.
 * The guard waits for the first emitted value from isAuthenticated$ which is
 * already resolved by the APP_INITIALIZER checkAuth() call.
 */
export const authGuard: CanActivateFn = () => {
  const oidcService = inject(OidcSecurityService);

  return oidcService.isAuthenticated$.pipe(
    take(1),
    map(({ isAuthenticated }) => {
      if (isAuthenticated) return true;
      oidcService.authorize();
      return false;
    })
  );
};
