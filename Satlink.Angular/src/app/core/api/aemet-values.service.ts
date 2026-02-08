import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { ApiResponse } from './api-response';
import { GetAemetValuesRequestDto, Request } from './aemet.types';

@Injectable({ providedIn: 'root' })
export class AemetValuesService {
  private readonly http = inject(HttpClient);

  getValues(payload: GetAemetValuesRequestDto) {
    const url = `${environment.baseApiUrl}/api/aemetvalues/values`;
    return this.http.post<ApiResponse<Request[]>>(url, payload);
  }
}
