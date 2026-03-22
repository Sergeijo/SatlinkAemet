import { PassedInitialConfig } from 'angular-auth-oidc-client';

import { environment } from '../../environments/environment';

export const authConfig: PassedInitialConfig = {
  config: {
    authority: environment.identityServerUrl,
    redirectUrl: `${environment.appUrl}/auth/callback`,
    postLogoutRedirectUri: environment.appUrl,
    clientId: 'satlink-angular',
    scope: 'openid profile email satlink-api',
    responseType: 'code',
    silentRenew: true,
    useRefreshToken: true,
    renewTimeBeforeTokenExpiresInSeconds: 30,
    // Attach the Bearer token to requests that start with any of these prefixes.
    // Covers both the Angular dev-proxy path (/api) and direct API calls.
    secureRoutes: ['/api', 'https://localhost:7273', 'http://localhost:5273'],
  }
};
