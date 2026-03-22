import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { switchMap } from 'rxjs/operators';

import { OidcSecurityService } from 'angular-auth-oidc-client';

import { environment } from '../../environments/environment';

/**
 * Functional HTTP interceptor that attaches the Duende IS Bearer token to every
 * outgoing request, except requests directed at the Identity Server itself
 * (discovery doc, token endpoint, etc.) which use their own credentials.
 *
 * getAccessToken() returns Observable<string> in angular-auth-oidc-client v19,
 * so we use switchMap to resolve it before cloning the request.
 */
export const satlinkAuthInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith(environment.identityServerUrl)) {
    return next(req);
  }

  const oidc = inject(OidcSecurityService);

  return oidc.getAccessToken().pipe(
    switchMap(token => {
      if (!token) {
        return next(req);
      }

      return next(
        req.clone({ headers: req.headers.set('Authorization', `Bearer ${token}`) })
      );
    })
  );
};
