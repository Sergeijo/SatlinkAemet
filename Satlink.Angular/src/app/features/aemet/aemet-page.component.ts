import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
    CommonModule,
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
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  readonly items = signal<Zona[]>([]);

  readonly canDownload = computed(() => !this.isLoading());

  download() {
    this.error.set(null);
    this.isLoading.set(true);
    this.items.set([]);

    this.apiKeyService.getApiKey().subscribe({
      next: (apiKey) => this.postValues(apiKey, true),
      error: (err) => {
        this.error.set(err instanceof Error ? err.message : 'Unable to obtain apiKey.');
        this.isLoading.set(false);
      }
    });
  }

  private postValues(apiKey: string, canRetryWithFreshKey: boolean) {
    const payload: GetAemetValuesRequestDto = {
      apiKey,
      url: environment.aemetUrl,
      zone: this.selectedZone()
    };

    this.aemetValues.getValues(payload).subscribe({
      next: (resp) => {
        const first = resp.data?.[0];
        const zonas = first?.prediccion?.zona ?? [];
        this.items.set(zonas);
        this.isLoading.set(false);
      },
      error: (err) => {
        const detail = (err?.error?.detail as string | undefined) ?? '';
        const message = detail || 'Request failed.';

        // If apiKey is invalid/expired and backend supports renewal, retry once.
        if (canRetryWithFreshKey && /api_key|apikey|caduc|expired|invalid/i.test(message)) {
          this.apiKeyService.invalidateCache();
          this.apiKeyService.getApiKey().subscribe({
            next: (freshKey) => this.postValues(freshKey, false),
            error: (e) => {
              this.error.set(e instanceof Error ? e.message : message);
              this.isLoading.set(false);
            }
          });
          return;
        }

        this.error.set(message);
        this.isLoading.set(false);
      }
    });
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
