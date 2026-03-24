# Módulo de Mapa con MapLibre GL

Este módulo implementa un mapa interactivo usando **MapLibre GL JS** con las últimas características de Angular (Signals, standalone components, etc.).

## Características

### 🗺️ Mapa Interactivo
- Mapa de Madrid con estilo base de MapLibre
- Controles de navegación (zoom in/out, rotación)
- Modo pantalla completa
- Vista 3D con inclinación (pitch: 45°)

### 🚗 Animación de Vehículo
- Vehículo animado moviéndose por una ruta circular en el centro de Madrid
- Rotación automática del icono según la dirección del movimiento
- Movimiento suave con interpolación lineal entre puntos
- Ruta que pasa por lugares emblemáticos:
  - Puerta del Sol
  - Gran Vía
  - Plaza de España
  - Templo de Debod
  - Moncloa
  - Chamartín
  - Salamanca
  - Retiro
  - Atocha
  - Lavapiés
  - La Latina

### 💡 Tooltip Interactivo
- Tooltip con información detallada del vehículo al pasar el ratón
- Diseño moderno con degradado y efectos de glassmorphism
- Información mostrada:
  - Marca
  - Modelo
  - Motor
  - Potencia
  - Combustible
  - Año
  - Color

## Arquitectura

### Componentes

#### `MapDialogComponent`
Componente standalone que maneja el diálogo y la lógica del mapa.

**Características principales:**
- Uso de Angular Signals para estado reactivo
- Gestión del ciclo de vida del mapa (inicialización y limpieza)
- Integración con PrimeNG Dialog
- ViewChild para acceso al contenedor del mapa

#### `VehicleService`
Servicio que gestiona la lógica de animación del vehículo.

**Métodos principales:**
- `getVehicleMovement$()`: Observable que emite posiciones actualizadas cada 100ms
- `getVehicleSpecs()`: Retorna las especificaciones técnicas del vehículo
- `calculateBearing()`: Calcula el ángulo de rotación basado en la dirección
- `interpolate()`: Interpolación suave entre puntos de la ruta

### Modelos

#### `Vehicle`
```typescript
interface Vehicle {
  id: string;
  specs: VehicleSpecs;
  position: VehiclePosition;
}
```

#### `VehicleSpecs`
```typescript
interface VehicleSpecs {
  marca: string;
  modelo: string;
  motor: string;
  potencia: string;
  combustible: string;
  año: number;
  color: string;
}
```

#### `VehiclePosition`
```typescript
interface VehiclePosition {
  lng: number;
  lat: number;
  bearing: number;
}
```

## Tecnologías

- **Angular 19**: Framework principal
- **MapLibre GL JS 4.7**: Librería de mapas
- **PrimeNG 19**: Componentes UI (Dialog, Button)
- **RxJS**: Programación reactiva
- **TypeScript 5.7**: Tipado estático
- **SCSS**: Estilos avanzados

## Patrones y Mejores Prácticas

### ✅ Signals de Angular
```typescript
readonly visible = signal(false);
readonly vehicleSpecs = signal<VehicleSpecs | null>(null);
readonly tooltipPosition = signal<{ x: number; y: number } | null>(null);
```

### ✅ Standalone Components
Todos los componentes son standalone, sin necesidad de NgModules.

### ✅ Reactive Programming
Uso de Observables para la animación del vehículo.

### ✅ Memory Management
- Limpieza adecuada de suscripciones
- Eliminación del mapa en `ngOnDestroy`
- Limpieza de markers y eventos

### ✅ Type Safety
Todo el código está completamente tipado con TypeScript.

### ✅ Component Communication
- Uso de `@ViewChild` para acceso al componente hijo
- Métodos públicos para control desde el padre

## Uso

```typescript
// En el componente padre
@ViewChild(MapDialogComponent) mapDialog!: MapDialogComponent;

showMap() {
  this.mapDialog.show();
}
```

```html
<!-- En el template del padre -->
<p-button
  label="Mostrar Mapa"
  icon="pi pi-map"
  severity="success"
  (onClick)="showMap()"
/>

<app-map-dialog />
```

## Personalización

### Cambiar la Ruta
Edita el array `madridRoute` en `VehicleService`:

```typescript
private readonly madridRoute: [number, number][] = [
  [-3.7038, 40.4168], // Punto 1
  [-3.7070, 40.4200], // Punto 2
  // ... más puntos
];
```

### Cambiar Velocidad de Animación
Modifica el intervalo en `getVehicleMovement$()`:

```typescript
return interval(50).pipe( // Más rápido (50ms)
  // ...
);
```

### Cambiar Estilo del Mapa
Modifica la URL del estilo en `initializeMap()`:

```typescript
style: 'https://tu-estilo-personalizado.json'
```

### Cambiar Datos del Vehículo
Edita el objeto `vehicle` en `VehicleService`.

## Dependencias

Asegúrate de tener instaladas las siguientes dependencias:

```bash
npm install maplibre-gl@^4.7.1
npm install --save-dev @types/maplibre-gl@^4.7.0
```

## Estructura de Archivos

```
features/
└── map/
    ├── components/
    │   ├── map-dialog.component.ts
    │   ├── map-dialog.component.html
    │   └── map-dialog.component.scss
    ├── services/
    │   └── vehicle.service.ts
    └── models/
        └── vehicle.model.ts
```

## Rendimiento

- ✅ Lazy loading del mapa (se inicializa solo al abrir el diálogo)
- ✅ Limpieza automática de recursos
- ✅ Uso eficiente de Signals para reactividad
- ✅ Animación suave con interpolación optimizada

## Compatibilidad

- Angular 19+
- Navegadores modernos con soporte para ES2022
- MapLibre GL JS 4.7+
