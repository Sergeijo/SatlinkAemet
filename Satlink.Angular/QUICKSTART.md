# Guía Rápida de Instalación y Ejecución

## Instalación

1. **Instalar dependencias**:
   ```bash
   cd Satlink.Angular
   npm install
   ```

   Esto instalará:
   - MapLibre GL v4.7.1
   - Todas las dependencias de Angular
   - Todas las dependencias de PrimeNG

## Ejecución en Desarrollo

```bash
npm start
```

El proyecto estará disponible en: http://localhost:4200

## Build para Producción

```bash
npm run build
```

Los archivos compilados estarán en: `dist/satlink.angular`

## Uso del Mapa

1. Navega a la página de AEMET en la aplicación
2. Verás tres botones:
   - **Descargar Datos**: Descarga los datos de AEMET
   - **Descargar JSON**: Descarga los datos en formato JSON
   - **Mostrar Mapa**: ⭐ **NUEVO** - Abre el mapa interactivo

3. Haz clic en **"Mostrar Mapa"**
4. Se abrirá un diálogo grande con el mapa de Madrid
5. Verás un coche (🚗) moviéndose por una ruta circular
6. Pasa el ratón sobre el coche para ver sus características

## Controles del Mapa

- **Zoom In/Out**: Usa los botones `+` y `-` o la rueda del ratón
- **Rotación**: Mantén presionado `Ctrl` (o `Cmd` en Mac) y arrastra
- **Inclinación**: Mantén presionado `Shift` y arrastra
- **Pantalla Completa**: Haz clic en el botón de pantalla completa
- **Arrastrar**: Haz clic y arrastra para mover el mapa

## Características del Vehículo

El vehículo animado muestra las siguientes características en el tooltip:

- **Marca**: Tesla
- **Modelo**: Model S
- **Motor**: Dual Motor
- **Potencia**: 670 CV
- **Combustible**: Eléctrico
- **Año**: 2024
- **Color**: Rojo

## Ruta del Vehículo

El vehículo sigue una ruta circular que pasa por los siguientes lugares de Madrid:

1. Puerta del Sol
2. Gran Vía
3. Plaza de España
4. Templo de Debod
5. Moncloa
6. Ciudad Universitaria
7. Chamartín
8. Salamanca
9. Retiro
10. Atocha
11. Lavapiés
12. La Latina

## Solución de Problemas

### El mapa no se muestra

1. Verifica que MapLibre GL está instalado:
   ```bash
   npm list maplibre-gl
   ```
   Debería mostrar: `maplibre-gl@4.7.1`

2. Limpia la caché de Angular:
   ```bash
   rm -rf .angular
   npm start
   ```

### El vehículo no se mueve

El vehículo comienza a moverse automáticamente cuando se abre el diálogo. Si no se mueve:

1. Cierra el diálogo y vuelve a abrirlo
2. Verifica la consola del navegador para errores
3. Asegúrate de que no hay errores de JavaScript

### El tooltip no aparece

1. Asegúrate de pasar el ratón directamente sobre el icono del coche
2. El tooltip aparece solo cuando el cursor está sobre el icono
3. Verifica que no hay errores en la consola

## Notas sobre SSR

El proyecto usa Server-Side Rendering (SSR) con Angular. Esto puede causar algunos warnings durante el build, pero no afecta la funcionalidad del mapa ya que:

1. MapLibre solo se inicializa en el navegador (client-side)
2. El componente del mapa se inicializa solo cuando se abre el diálogo
3. Toda la lógica del mapa está protegida para ejecutarse solo en el cliente

## Personalización

Para personalizar el vehículo, la ruta o el estilo del mapa, consulta el archivo:
`MAPA_IMPLEMENTATION.md`

## Soporte

Para más información sobre MapLibre GL:
- Documentación: https://maplibre.org/maplibre-gl-js/docs/
- Ejemplos: https://maplibre.org/maplibre-gl-js/docs/examples/

Para más información sobre Angular Signals:
- Documentación: https://angular.dev/guide/signals
