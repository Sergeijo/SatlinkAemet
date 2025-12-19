# Copilot instructions

Sigue las reglas maestras definidas en `.github/instructions/rules.instructions.md`.

## Estilo y convenciones
- Allman style: llaves en nueva línea.
- Interfaces con prefijo `I`.
- Servicios con sufijo `Service`.
- Métodos `async` con sufijo `Async`.
- Campos privados `_camelCase`.
- Evitar `var` salvo cuando el tipo sea obvio.

## API
- No incluir lógica de negocio en controladores.
- Respuestas con `Result<T>` o `ApiResponse<T>`.
- Errores con RFC 7807 (`ProblemDetails`).
- Logging estructurado con `ILogger`.

## Validación
- Validaciones de entrada con FluentValidation.

## Documentación
- XML docs obligatorios en métodos públicos.
- Comentarios inline para pasos de lógica.

## Testing
- xUnit + NSubstitute.
- Arrange-Act-Assert.
- Nombres de tests: `MetodoTesteado_Escenario_ResultadoEsperado`.
