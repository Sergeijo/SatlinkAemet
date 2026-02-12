
import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { catchError, concat, defer, map, Observable, of, skip, switchMap } from 'rxjs';

import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';

import { environment } from '../../../environments/environment';
import { AemetApiKeyService } from '../../core/api/aemet-api-key.service';
import { AemetValuesService } from '../../core/api/aemet-values.service';
import { GetAemetValuesRequestDto, Zona } from '../../core/api/aemet.types';

type ZoneOption = { key: number; value: string };

@Component({
  selector: 'app-aemet-page',
  standalone: true,
  imports: [
    FormsModule,
    SelectModule,
    ButtonModule,
    TableModule,
    ProgressSpinnerModule,
    MessageModule
],
  templateUrl: './aemet-page.component.html',
  styleUrl: './aemet-page.component.scss'
})
export class AemetPageComponent {
  private readonly apiKeyService = inject(AemetApiKeyService);
  private readonly aemetValues = inject(AemetValuesService);

  readonly zones: ZoneOption[] = [
    { key: 0, value: 'Océano Atlántico al sur de 35º N' },
    { key: 1, value: 'Océano Atlántico al norte de 30º N' },
    { key: 2, value: 'Mar Mediterráneo' }
  ];

  readonly selectedZone = signal<number>(2);
  private readonly downloadTrigger = signal(0);

  private readonly state = toSignal(this.createLoadState$(), {
    initialValue: { loading: false, error: null as string | null, items: [] as Zona[] }
  });

  readonly isLoading = computed(() => this.state().loading);
  readonly error = computed(() => this.state().error);
  readonly items = computed(() => this.state().items);
  readonly canDownload = computed(() => !this.isLoading());

  download() {
    this.downloadTrigger.update(v => v + 1);
  }

  private createLoadState$(): Observable<{ loading: boolean; error: string | null; items: Zona[] }> {
    return toObservable(this.downloadTrigger).pipe(
      skip(1),
      switchMap(() => {
        const zone = this.selectedZone();

        const request$ = defer(() => this.apiKeyService.getApiKey()).pipe(
          switchMap((apiKey) => this.fetchZonas$(apiKey, zone, true)),
          map((items) => ({ loading: false, error: null, items })),
          catchError((err: unknown) => {
            const message = err instanceof Error ? err.message : 'Request failed.';
            return of({ loading: false, error: message, items: [] as Zona[] });
          })
        );

        return concat(
          of({ loading: true, error: null, items: [] as Zona[] }),
          request$
        );
      })
    );
  }

  private fetchZonas$(apiKey: string, zone: number, canRetryWithFreshKey: boolean): Observable<Zona[]> {
    const payload: GetAemetValuesRequestDto = {
      apiKey,
      url: environment.aemetUrl,
      zone
    };

    return this.aemetValues.getValues(payload).pipe(
      map((resp) => {
        const first = resp.data?.[0];
        return first?.prediccion?.zona ?? [];
      }),
      catchError((err: any) => {
        const detail = (err?.error?.detail as string | undefined) ?? '';
        const message = detail || 'Request failed.';

        // If apiKey is invalid/expired and backend supports renewal, retry once.
        if (canRetryWithFreshKey && /api_key|apikey|caduc|expired|invalid/i.test(message)) {
          this.apiKeyService.invalidateCache();
          return this.apiKeyService.getApiKey().pipe(
            switchMap((freshKey) => this.fetchZonas$(freshKey, zone, false))
          );
        }

        return defer(() => {
          throw new Error(message);
        });
      })
    );
  }

  downloadJsonFile() {
    const data = this.items();
    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json;charset=utf-8' });
    const url = URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = `aemet-zone-${this.selectedZone()}-${new Date().toISOString()}.json`;
    a.click();

    URL.revokeObjectURL(url);
  }

  trackByZonaId(_index: number, item: Zona) {
    return item.id;
  }
}
