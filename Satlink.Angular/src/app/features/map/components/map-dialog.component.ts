import {
  Component,
  ElementRef,
  OnInit,
  OnDestroy,
  ViewChild,
  inject,
  signal,
  effect,
  AfterViewInit
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import maplibregl from 'maplibre-gl';
import { Subscription } from 'rxjs';
import { VehicleService } from '../services/vehicle.service';
import { VehicleSpecs } from '../models/vehicle.model';

@Component({
  selector: 'app-map-dialog',
  standalone: true,
  imports: [CommonModule, DialogModule, ButtonModule],
  templateUrl: './map-dialog.component.html',
  styleUrl: './map-dialog.component.scss'
})
export class MapDialogComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapContainer', { static: false }) mapContainer!: ElementRef<HTMLDivElement>;

  private readonly vehicleService = inject(VehicleService);
  
  readonly visible = signal(false);
  readonly vehicleSpecs = signal<VehicleSpecs | null>(null);
  readonly tooltipPosition = signal<{ x: number; y: number } | null>(null);

  private map: maplibregl.Map | null = null;
  private vehicleMarker: maplibregl.Marker | null = null;
  private movementSubscription: Subscription | null = null;

  constructor() {
    effect(() => {
      if (this.visible()) {
        this.vehicleSpecs.set(this.vehicleService.getVehicleSpecs());
      }
    });
  }

  ngAfterViewInit(): void {
    // Map initialization will happen when dialog opens
  }

  ngOnDestroy(): void {
    this.cleanup();
  }

  show(): void {
    this.visible.set(true);
    setTimeout(() => this.initializeMap(), 100);
  }

  hide(): void {
    this.visible.set(false);
    this.cleanup();
  }

  private initializeMap(): void {
    if (!this.mapContainer || this.map) return;

    this.map = new maplibregl.Map({
      container: this.mapContainer.nativeElement,
      style: {
        version: 8,
        sources: {
          'osm': {
            type: 'raster',
            tiles: ['https://tile.openstreetmap.org/{z}/{x}/{y}.png'],
            tileSize: 256,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
          }
        },
        layers: [
          {
            id: 'osm',
            type: 'raster',
            source: 'osm',
            minzoom: 0,
            maxzoom: 19
          }
        ]
      },
      center: [-3.7038, 40.4168],
      zoom: 13,
      pitch: 0,
      bearing: 0,
      antialias: true
    });

    this.map.addControl(new maplibregl.NavigationControl(), 'top-right');
    this.map.addControl(new maplibregl.FullscreenControl(), 'top-right');

    this.map.on('load', () => {
      console.log('Map loaded successfully');
      this.setupVehicleMarker();
      this.startVehicleAnimation();
    });

    this.map.on('error', (e) => {
      console.error('Map error:', e);
    });
  }

  private setupVehicleMarker(): void {
    if (!this.map) return;

    const el = document.createElement('div');
    el.className = 'vehicle-marker';
    el.innerHTML = '🚗';
    el.style.fontSize = '32px';
    el.style.cursor = 'pointer';
    el.style.width = '40px';
    el.style.height = '40px';
    el.style.display = 'flex';
    el.style.alignItems = 'center';
    el.style.justifyContent = 'center';

    el.addEventListener('mouseenter', (e: MouseEvent) => {
      const rect = el.getBoundingClientRect();
      this.tooltipPosition.set({
        x: rect.left + rect.width / 2,
        y: rect.top
      });
    });

    el.addEventListener('mouseleave', () => {
      this.tooltipPosition.set(null);
    });

    this.vehicleMarker = new maplibregl.Marker({
      element: el,
      anchor: 'center'
    })
      .setLngLat([-3.7038, 40.4168])
      .addTo(this.map);

    console.log('Vehicle marker created at:', [-3.7038, 40.4168]);
  }

  private startVehicleAnimation(): void {
    this.movementSubscription = this.vehicleService.getVehicleMovement$()
      .subscribe(position => {
        if (this.vehicleMarker && this.map) {
          const lngLat: [number, number] = [position.lng, position.lat];
          this.vehicleMarker.setLngLat(lngLat);

          // Rotar solo el contenido del marker (el div con clase vehicle-marker)
          // NO el contenedor .maplibregl-marker que MapLibre usa para posicionar
          const markerElement = this.vehicleMarker.getElement();
          const vehicleIcon = markerElement.querySelector('.vehicle-marker') as HTMLElement;
          if (vehicleIcon) {
            vehicleIcon.style.transform = `rotate(${position.bearing}deg)`;
          }

          console.log('Vehicle position:', lngLat, 'bearing:', position.bearing);
        }
      });
  }

  private cleanup(): void {
    this.movementSubscription?.unsubscribe();
    this.movementSubscription = null;

    if (this.vehicleMarker) {
      this.vehicleMarker.remove();
      this.vehicleMarker = null;
    }

    if (this.map) {
      this.map.remove();
      this.map = null;
    }

    this.tooltipPosition.set(null);
  }

  onDialogHide(): void {
    this.cleanup();
  }
}
