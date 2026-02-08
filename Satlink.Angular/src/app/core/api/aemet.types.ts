export interface Origen {
  productor?: string;
  web?: string;
  language?: string;
  copyright?: string;
  notaLegal?: string;
  elaborado?: string;
  inicio?: string;
  fin?: string;
}

export interface Zona {
  texto?: string;
  id: number;
  nombre?: string;
}

export interface Prediccion {
  inicio?: string;
  fin?: string;
  zona?: Zona[];
}

export interface Situacion {
  inicio?: string;
  fin?: string;
  texto?: string;
  id?: string;
  nombre?: string;
}

export interface Request {
  origen?: Origen;
  situacion?: Situacion;
  prediccion?: Prediccion;
  id?: string;
  nombre?: string;
}

export interface GetAemetValuesRequestDto {
  apiKey: string;
  url: string;
  zone: number;
}

export interface AemetApiKeyResponseDto {
  apiKey: string;
  expiresAtUtc?: string;
}
