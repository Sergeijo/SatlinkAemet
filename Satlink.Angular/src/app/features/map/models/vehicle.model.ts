export interface VehicleSpecs {
  marca: string;
  modelo: string;
  motor: string;
  potencia: string;
  combustible: string;
  anio: number;
  color: string;
}

export interface VehiclePosition {
  lng: number;
  lat: number;
  bearing: number;
}

export interface Vehicle {
  id: string;
  specs: VehicleSpecs;
  position: VehiclePosition;
}
