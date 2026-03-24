# 📁 Estructura del Proyecto - Feature Map

```
Satlink.Angular/
│
├── 📦 package.json                    [MODIFICADO] ← maplibre-gl@4.7.1
├── ⚙️ angular.json                     [MODIFICADO] ← Estilos de MapLibre
│
├── 📖 MAPA_IMPLEMENTATION.md          [NUEVO] ← Documentación completa
├── 📖 QUICKSTART.md                   [NUEVO] ← Guía rápida
├── 📖 IMPLEMENTACION_COMPLETADA.md    [NUEVO] ← Resumen ejecutivo
│
└── src/
    ├── 🎨 styles.scss                 [MODIFICADO] ← Limpieza
    │
    └── app/
        └── features/
            │
            ├── 📊 aemet/              [MODIFICADO]
            │   ├── aemet-page.component.ts     ← Integración MapDialogComponent
            │   ├── aemet-page.component.html   ← Botón "Mostrar Mapa"
            │   └── aemet-page.component.scss
            │
            └── 🗺️ map/                [NUEVO - TODO EL DIRECTORIO]
                │
                ├── 📄 index.ts        ← Barrel exports
                ├── 📖 README.md       ← Documentación técnica
                │
                ├── 📐 models/
                │   └── vehicle.model.ts
                │       ├── Vehicle
                │       ├── VehicleSpecs
                │       └── VehiclePosition
                │
                ├── ⚙️ services/
                │   └── vehicle.service.ts
                │       ├── getVehicleMovement$()
                │       ├── getVehicleSpecs()
                │       ├── calculateBearing()
                │       └── interpolate()
                │
                └── 🎨 components/
                    ├── map-dialog.component.ts
                    │   ├── show()
                    │   ├── hide()
                    │   ├── initializeMap()
                    │   ├── setupVehicleMarker()
                    │   ├── startVehicleAnimation()
                    │   └── cleanup()
                    │
                    ├── map-dialog.component.html
                    │   ├── <p-dialog>
                    │   ├── #mapContainer
                    │   └── Tooltip condicional
                    │
                    └── map-dialog.component.scss
                        ├── .map-container
                        ├── .vehicle-marker
                        ├── .vehicle-tooltip
                        └── Animaciones
```

## 🔄 Flujo de Datos

```
Usuario                AemetPageComponent          MapDialogComponent           VehicleService
  │                           │                           │                           │
  │  Clic "Mostrar Mapa"      │                           │                           │
  ├──────────────────────────>│                           │                           │
  │                           │  show()                   │                           │
  │                           ├──────────────────────────>│                           │
  │                           │                           │  initializeMap()          │
  │                           │                           ├───────────────┐           │
  │                           │                           │               │           │
  │                           │                           │<──────────────┘           │
  │                           │                           │  setupVehicleMarker()     │
  │                           │                           ├───────────────┐           │
  │                           │                           │               │           │
  │                           │                           │<──────────────┘           │
  │                           │                           │  getVehicleMovement$()    │
  │                           │                           ├──────────────────────────>│
  │                           │                           │                           │
  │                           │                           │  Observable<Position>     │
  │                           │                           │<──────────────────────────┤
  │                           │                           │  (cada 100ms)             │
  │                           │                           │                           │
  │  Mouse Over Vehicle       │                           │                           │
  ├──────────────────────────────────────────────────────>│                           │
  │                           │                           │  getVehicleSpecs()        │
  │                           │                           ├──────────────────────────>│
  │                           │                           │                           │
  │                           │                           │  VehicleSpecs             │
  │  Tooltip Visible          │<──────────────────────────┤<──────────────────────────┤
  │<──────────────────────────────────────────────────────┤                           │
  │                           │                           │                           │
```

## 🎯 Componentes Clave

### 1️⃣ VehicleService
```typescript
📍 Ubicación: services/vehicle.service.ts
🎯 Propósito: Gestionar estado y animación del vehículo
🔧 Tecnologías: Angular Signals + RxJS
📊 Estado: Signal<Vehicle>
🔄 Stream: Observable<VehiclePosition> (100ms)
```

### 2️⃣ MapDialogComponent
```typescript
📍 Ubicación: components/map-dialog.component.ts
🎯 Propósito: Diálogo con mapa interactivo
🔧 Tecnologías: MapLibre GL + Angular Signals
📊 Estado: visible, vehicleSpecs, tooltipPosition
🗺️ Mapa: MapLibre GL Map
🚗 Marker: Custom HTML Marker
```

### 3️⃣ AemetPageComponent
```typescript
📍 Ubicación: aemet/aemet-page.component.ts
🎯 Propósito: Página principal de AEMET
🔧 Integración: ViewChild<MapDialogComponent>
🔘 Botón: "Mostrar Mapa" → mapDialog.show()
```

## 🧩 Interfaces TypeScript

### Vehicle
```typescript
{
  id: string
  specs: VehicleSpecs
  position: VehiclePosition
}
```

### VehicleSpecs
```typescript
{
  marca: string
  modelo: string
  motor: string
  potencia: string
  combustible: string
  anio: number
  color: string
}
```

### VehiclePosition
```typescript
{
  lng: number
  lat: number
  bearing: number
}
```

## 🎨 Estilos Principales

### Tooltip Moderno
- Gradiente púrpura (667eea → 764ba2)
- Glassmorphism (blur + transparencia)
- Animación fadeIn
- Grid layout para información

### Marker del Vehículo
- Emoji 🚗 de 32px
- Drop shadow para profundidad
- Transición suave en hover
- Rotación dinámica

### Diálogo
- 90vw × 85vh (responsive)
- Mapa: 70vh
- Header con gradiente
- Controles estilizados

## 📊 Ruta del Vehículo

```
[Inicio] Puerta del Sol (-3.7038, 40.4168)
    ↓
Gran Vía (-3.7070, 40.4200)
    ↓
Plaza de España (-3.7100, 40.4230)
    ↓
Templo de Debod (-3.7120, 40.4260)
    ↓
Moncloa (-3.7080, 40.4290)
    ↓
Ciudad Universitaria (-3.7000, 40.4310)
    ↓
Chamartín (-3.6920, 40.4280)
    ↓
Salamanca (-3.6850, 40.4240)
    ↓
Retiro (-3.6820, 40.4200)
    ↓
Atocha (-3.6850, 40.4160)
    ↓
Lavapiés (-3.6920, 40.4130)
    ↓
La Latina (-3.7000, 40.4120)
    ↓
[Vuelta] Puerta del Sol (-3.7038, 40.4168)
```

## 🔄 Ciclo de Vida

```
Usuario hace clic
    ↓
visible.set(true)
    ↓
setTimeout → initializeMap()
    ↓
new maplibregl.Map()
    ↓
map.on('load')
    ↓
setupVehicleMarker()
    ↓
startVehicleAnimation()
    ↓
subscribe(getVehicleMovement$())
    ↓
[Loop] cada 100ms:
    - Calcular nueva posición
    - Interpolar coordenadas
    - Calcular bearing
    - Actualizar marker
    - Rotar icono
    ↓
Usuario cierra diálogo
    ↓
cleanup()
    - unsubscribe()
    - marker.remove()
    - map.remove()
```

## 📦 Dependencias

```json
{
  "maplibre-gl": "^4.7.1"  ← Principal
}
```

### Dependencias de Angular (ya existentes)
- @angular/core
- @angular/common
- rxjs
- primeng

## 🎯 Patrones Utilizados

✅ **Singleton Service**: `providedIn: 'root'`
✅ **Observer Pattern**: RxJS Observables
✅ **Reactive Programming**: Signals + Effects
✅ **Component Composition**: Parent-Child con ViewChild
✅ **Separation of Concerns**: Service → Component → Template
✅ **Lazy Initialization**: Mapa solo cuando se necesita
✅ **Resource Management**: Cleanup en ngOnDestroy
✅ **Type Safety**: Interfaces TypeScript estrictas

## 🚀 Performance

- ✅ Lazy Loading del mapa
- ✅ Limpieza automática de recursos
- ✅ Interpolación eficiente
- ✅ Actualización cada 100ms (óptimo)
- ✅ Signals para reactividad granular
- ✅ No re-renders innecesarios

---

**¡Todo Implementado y Documentado!** 🎉
