import { Injectable, signal } from '@angular/core';
import { interval, map, Observable } from 'rxjs';
import { Vehicle, VehiclePosition, VehicleSpecs } from '../models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly vehicle = signal<Vehicle>({
    id: 'vehicle-001',
    specs: {
      marca: 'Tesla',
      modelo: 'Model S',
      motor: 'Dual Motor',
      potencia: '670 CV',
      combustible: 'Eléctrico',
      anio: 2024,
      color: 'Rojo'
    },
    position: {
      lng: -3.7038,
      lat: 40.4168,
      bearing: 0
    }
  });

  // Ruta circular por el centro de Madrid
  private readonly madridRoute: [number, number][] = [
    [-3.7038, 40.4168], // Puerta del Sol
    [-3.7070, 40.4200], // Gran Vía
    [-3.7100, 40.4230], // Plaza de España
    [-3.7120, 40.4260], // Templo de Debod
    [-3.7080, 40.4290], // Moncloa
    [-3.7000, 40.4310], // Ciudad Universitaria
    [-3.6920, 40.4280], // Chamartín
    [-3.6850, 40.4240], // Salamanca
    [-3.6820, 40.4200], // Retiro
    [-3.6850, 40.4160], // Atocha
    [-3.6920, 40.4130], // Lavapiés
    [-3.7000, 40.4120], // La Latina
    [-3.7038, 40.4168]  // Vuelta a Puerta del Sol
  ];

  private currentIndex = 0;
  private progress = 0;

  readonly currentVehicle = this.vehicle.asReadonly();

  /**
   * Retorna un Observable que emite la posición actualizada del vehículo
   * cada 100ms, moviéndose suavemente a lo largo de la ruta
   */
  getVehicleMovement$(): Observable<VehiclePosition> {
    return interval(100).pipe(
      map(() => {
        this.progress += 0.02;

        if (this.progress >= 1) {
          this.progress = 0;
          this.currentIndex = (this.currentIndex + 1) % (this.madridRoute.length - 1);
        }

        const start = this.madridRoute[this.currentIndex];
        const end = this.madridRoute[this.currentIndex + 1];

        const lng = this.interpolate(start[0], end[0], this.progress);
        const lat = this.interpolate(start[1], end[1], this.progress);
        const bearing = this.calculateBearing(start, end);

        const position: VehiclePosition = { lng, lat, bearing };

        this.vehicle.update(v => ({
          ...v,
          position
        }));

        return position;
      })
    );
  }

  getVehicleSpecs(): VehicleSpecs {
    return this.vehicle().specs;
  }

  private interpolate(start: number, end: number, progress: number): number {
    return start + (end - start) * progress;
  }

  private calculateBearing(start: [number, number], end: [number, number]): number {
    const [lng1, lat1] = start;
    const [lng2, lat2] = end;

    const dLng = (lng2 - lng1) * Math.PI / 180;
    const lat1Rad = lat1 * Math.PI / 180;
    const lat2Rad = lat2 * Math.PI / 180;

    const y = Math.sin(dLng) * Math.cos(lat2Rad);
    const x = Math.cos(lat1Rad) * Math.sin(lat2Rad) -
              Math.sin(lat1Rad) * Math.cos(lat2Rad) * Math.cos(dLng);

    const bearing = Math.atan2(y, x);
    return (bearing * 180 / Math.PI + 360) % 360;
  }
}
