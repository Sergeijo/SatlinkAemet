# Implementación del Botón "Mostrar Mapa" con MapLibre GL

## Resumen de Cambios

Se ha implementado un sistema completo de visualización de mapas interactivos usando **MapLibre GL JS** con las últimas características de **Angular 19** (Signals, standalone components, reactive programming).

## Archivos Creados

### 1. Modelos de Datos
- **`Satlink.Angular/src/app/features/map/models/vehicle.model.ts`**
  - Interfaces TypeScript para Vehicle, VehicleSpecs y VehiclePosition
  - Tipado completo para todas las propiedades del vehículo

### 2. Servicio de Vehículo
- **`Satlink.Angular/src/app/features/map/services/vehicle.service.ts`**
  - Servicio providedIn: 'root' con gestión de estado usando Signals
  - Observable reactivo que emite posiciones del vehículo cada 100ms
  - Ruta circular por 12 puntos emblemáticos de Madrid
  - Cálculo de bearing (orientación) basado en fórmulas geoespaciales
  - Interpolación suave para animación fluida

### 3. Componente de Diálogo del Mapa
- **`Satlink.Angular/src/app/features/map/components/map-dialog.component.ts`**
  - Componente standalone con integración de MapLibre GL
  - Uso de Signals para estado reactivo (visible, vehicleSpecs, tooltipPosition)
  - Gestión completa del ciclo de vida del mapa
  - Implementación de marker personalizado con rotación
  - Event listeners para mostrar/ocultar tooltip
  - Limpieza automática de recursos (subscriptions, map, markers)

- **`Satlink.Angular/src/app/features/map/components/map-dialog.component.html`**
  - PrimeNG Dialog con tamaño responsive (90vw x 85vh)
  - Contenedor del mapa con referencia #mapContainer
  - Tooltip posicionado absolutamente con información del vehículo
  - Uso de @if con Signals para renderizado condicional

- **`Satlink.Angular/src/app/features/map/components/map-dialog.component.scss`**
  - Estilos modernos con gradientes y glassmorphism
  - Animaciones CSS para el tooltip (fadeIn)
  - Estilos para el marker del vehículo con efectos hover
  - Diseño responsive y accesible
  - Personalización de controles de MapLibre

### 4. Documentación
- **`Satlink.Angular/src/app/features/map/README.md`**
  - Documentación completa del módulo
  - Explicación de arquitectura y patrones
  - Guías de uso y personalización
  - Listado de dependencias y compatibilidad

- **`Satlink.Angular/src/app/features/map/index.ts`**
  - Archivo de barril para facilitar importaciones

## Archivos Modificados

### 1. Package.json
- **`Satlink.Angular/package.json`**
  - Agregado: `"maplibre-gl": "^4.7.1"` en dependencies
  - MapLibre GL JS es la librería de mapas de código abierto

### 2. Estilos Globales
- **`Satlink.Angular/src/styles.scss`**
  - Agregado import de estilos de MapLibre: `@import "maplibre-gl/dist/maplibre-gl.css";`
  - Necesario para que los controles y elementos del mapa se vean correctamente

### 3. Componente Aemet Page
- **`Satlink.Angular/src/app/features/aemet/aemet-page.component.ts`**
  - Agregado import de MapDialogComponent
  - Agregado ViewChild para acceder al componente del mapa
  - Agregado MapDialogComponent en el array de imports
  - Nuevo método `showMap()` que llama a `mapDialog.show()`

- **`Satlink.Angular/src/app/features/aemet/aemet-page.component.html`**
  - Agregado botón "Mostrar Mapa" con icono `pi-map` y severity="success"
  - Agregado componente `<app-map-dialog />` al final del template
  - Botón posicionado al lado del botón "Descargar JSON"

## Características Implementadas

### ✅ Mapa Interactivo
- Mapa centrado en Madrid (Puerta del Sol)
- Zoom inicial: 13
- Vista 3D con inclinación (pitch: 45°)
- Controles de navegación (zoom, rotación)
- Control de pantalla completa
- Estilo base de MapLibre (demotiles)

### ✅ Animación de Vehículo
- Icono de coche (🚗) de 32px
- Movimiento suave por ruta circular de 12 puntos
- Rotación automática según dirección de movimiento
- Velocidad configurable (actualmente 100ms por frame)
- Interpolación lineal entre puntos
- Ruta que cubre el centro de Madrid

### ✅ Tooltip Interactivo
- Aparece al pasar el ratón sobre el vehículo
- Desaparece al salir del área del vehículo
- Diseño moderno con gradiente púrpura
- Efecto glassmorphism (blur + transparencia)
- Animación fadeIn suave
- Información completa del vehículo:
  - Marca: Tesla
  - Modelo: Model S
  - Motor: Dual Motor
  - Potencia: 670 CV
  - Combustible: Eléctrico
  - Año: 2024
  - Color: Rojo

### ✅ Integración UI
- Botón "Mostrar Mapa" con icono de mapa
- Color verde (severity="success") para destacar
- Diálogo modal responsive
- Se puede cerrar con el botón X o ESC

## Tecnologías y Patrones Utilizados

### Angular 19 Features
- ✅ **Signals**: Estado reactivo sin necesidad de RxJS
- ✅ **Standalone Components**: Sin NgModules
- ✅ **ViewChild**: Comunicación padre-hijo
- ✅ **@if syntax**: Nueva sintaxis de control de flujo
- ✅ **effect()**: Reacción a cambios en Signals

### Reactive Programming
- ✅ **Observables (RxJS)**: Para la animación del vehículo
- ✅ **interval()**: Emisiones cada 100ms
- ✅ **map()**: Transformación de datos
- ✅ **Subscription management**: Limpieza adecuada

### Best Practices
- ✅ **Type Safety**: Todo el código está tipado
- ✅ **Memory Management**: Limpieza en ngOnDestroy
- ✅ **Lazy Loading**: El mapa se inicializa solo al abrirse
- ✅ **Separation of Concerns**: Servicio separado para lógica de negocio
- ✅ **Component Communication**: ViewChild para control desde padre

### MapLibre GL Features
- ✅ **Marker API**: Marcador personalizado con HTML
- ✅ **Map Controls**: Navigation y Fullscreen
- ✅ **Event Handling**: Mouse events en el marker
- ✅ **3D Visualization**: Pitch y bearing para vista angular

## Cómo Usar

1. **Instalar dependencias**:
   ```bash
   cd Satlink.Angular
   npm install
   ```

2. **Ejecutar el proyecto**:
   ```bash
   npm start
   ```

3. **Navegar a la página de AEMET**
   - La página ya debe estar disponible en tu aplicación

4. **Hacer clic en "Mostrar Mapa"**
   - El botón está al lado de "Descargar JSON"
   - Se abrirá un diálogo grande con el mapa
   - El coche comenzará a moverse automáticamente
   - Pasa el ratón sobre el coche para ver sus características

## Personalización Recomendada

### Cambiar el vehículo
Edita `VehicleService.vehicle`:
```typescript
specs: {
  marca: 'Tu Marca',
  modelo: 'Tu Modelo',
  // ...
}
```

### Cambiar la ruta
Edita `VehicleService.madridRoute` con tus propias coordenadas.

### Cambiar velocidad
Modifica el `interval(100)` en `getVehicleMovement$()`.

### Cambiar estilo del mapa
Puedes usar otros estilos de MapLibre o crear el tuyo:
```typescript
style: 'https://tu-estilo.json'
```

### Cambiar icono del vehículo
En `setupVehicleMarker()`:
```typescript
el.innerHTML = '🚕'; // O cualquier emoji/HTML
```

## Estructura Final de Carpetas

```
Satlink.Angular/
└── src/
    └── app/
        └── features/
            ├── aemet/
            │   ├── aemet-page.component.ts       [MODIFICADO]
            │   ├── aemet-page.component.html     [MODIFICADO]
            │   └── aemet-page.component.scss
            └── map/                               [NUEVO]
                ├── components/
                │   ├── map-dialog.component.ts
                │   ├── map-dialog.component.html
                │   └── map-dialog.component.scss
                ├── services/
                │   └── vehicle.service.ts
                ├── models/
                │   └── vehicle.model.ts
                ├── README.md
                └── index.ts
```

## Ventajas de esta Implementación

1. **Código Moderno**: Usa las últimas características de Angular 19
2. **Mantenible**: Código bien organizado y documentado
3. **Performante**: Lazy loading y limpieza adecuada de recursos
4. **Escalable**: Fácil agregar más vehículos o rutas
5. **Testeable**: Servicios y componentes independientes
6. **Responsive**: Se adapta a diferentes tamaños de pantalla
7. **Accesible**: Usa semántica HTML adecuada

## Próximos Pasos Sugeridos

1. **Agregar múltiples vehículos**: Modificar el servicio para manejar un array
2. **Datos en tiempo real**: Conectar con API de tracking real
3. **Rutas dinámicas**: Permitir al usuario definir la ruta
4. **Más información**: Agregar velocidad actual, tiempo estimado, etc.
5. **Estilos de mapa**: Permitir al usuario elegir el estilo del mapa
6. **Exportar ruta**: Botón para exportar la ruta como GPX o KML
7. **Tests unitarios**: Agregar tests para servicios y componentes

## Dependencias Instaladas

```json
{
  "maplibre-gl": "^4.7.1"
}
```

Esta librería es de código abierto, gratuita y muy activa en su desarrollo.

## Compatibilidad

- ✅ Angular 19+
- ✅ TypeScript 5.7+
- ✅ Navegadores modernos (Chrome, Firefox, Safari, Edge)
- ✅ Compatible con SSR (Server-Side Rendering)
- ✅ Compatible con dispositivos móviles

---

**¡Implementación Completa y Lista para Usar!** 🎉
