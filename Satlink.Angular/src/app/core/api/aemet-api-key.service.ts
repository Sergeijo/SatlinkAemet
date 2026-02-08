import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, map, Observable, of, shareReplay, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from './api-response';
import { AemetApiKeyResponseDto } from './aemet.types';

@Injectable({ providedIn: 'root' })
export class AemetApiKeyService {
  private readonly http = inject(HttpClient);

  private cached$?: Observable<string>;

  getApiKey(): Observable<string> {
    if (environment.apiKey?.trim()) {
      return of(environment.apiKey.trim());
    }

    if (!this.cached$) {
      this.cached$ = this.fetchApiKey().pipe(shareReplay({ bufferSize: 1, refCount: true }));
    }

    return this.cached$;
  }

  invalidateCache() {
    this.cached$ = undefined;
  }

  private fetchApiKey(): Observable<string> {
    const url = `${environment.baseApiUrl}/api/aemetvalues/apikey`;

    return this.http.get<ApiResponse<AemetApiKeyResponseDto>>(url).pipe(
      map(r => r.data?.apiKey?.trim() ?? ''),
      map(key => {
        if (!key) {
          throw new Error('Empty apiKey returned from server.');
        }
        return key;
      }),
      catchError((err: unknown) => {
        // If backend does not expose the endpoint, provide a clearer error.
        if (err instanceof HttpErrorResponse && err.status === 404) {
          return throwError(() => new Error('Missing endpoint GET /api/aemetvalues/apikey in Satlink.Api. Configure environment.apiKey or implement the endpoint.'));
        }
        return throwError(() => err);
      })
    );
  }
}
