# Reglas maestras del proyecto

Estas reglas aplican a todo el código del workspace.

## Convenciones de código

- Interfaces siempre con prefijo `I`.
- Servicios siempre con sufijo `Service`.
- Métodos `async` siempre con sufijo `Async`.
- Usar `var` solo cuando el tipo sea obvio.
- Estilo de llaves Allman (llave en nueva línea).
- Campos privados en `_camelCase`.

## Documentación

- Comentarios XML obligatorios en todos los métodos públicos:
  - `/// <summary>`
  - `/// <param>`
  - `/// <returns>`
  - `/// <exception>` cuando aplique.
- Comentarios inline para describir pasos relevantes de la lógica.

## API

- Las respuestas deben envolverse en `Result<T>` o `ApiResponse<T>`.
- Errores deben seguir RFC 7807 (`ProblemDetails`).
- Logging estructurado con `ILogger`.
- Prohibido `Console.WriteLine` en producción.

## Validaciones

- Validaciones de DTO de entrada deben implementarse con FluentValidation.

## Testing

- Tests con xUnit y NSubstitute.
- Patrón Arrange-Act-Assert.
- Nombre de tests: `MetodoTesteado_Escenario_ResultadoEsperado`.
