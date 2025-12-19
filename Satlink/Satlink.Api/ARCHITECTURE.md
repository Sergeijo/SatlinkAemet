# Satlink.Api - Architecture

## Objetivo

`Satlink.Api` implementa la capa **Presentation** de la solución:
- Traduce HTTP/JSON a DTOs.
- Valida entradas.
- Orquesta llamadas a `Satlink.Logic`.
- Devuelve respuestas uniformes (`ApiResponse<T>` y `ProblemDetails`).

## Módulos

- `Controllers/`
  - Endpoints REST.
  - Sin lógica de negocio.

- `Dtos/`
  - Contratos de entrada/salida para HTTP.

- `Validation/`
  - Validaciones con FluentValidation.

- `Services/`
  - Servicios propios de la API (p.ej. `TokenService`) que no pertenecen a reglas de negocio.

- `Contracts/`
  - `ApiResponse<T>`, `PagedResult<T>`, helpers RFC 7807.

- `Utilities/`
  - `PaginationHelper`.

## Flujo de solicitud (diagrama)

```text
Client
  |
  | HTTP request
  v
Controller (Satlink.Api)
  |
  | DTO binding
  v
FluentValidation
  |
  | if invalid -> ValidationProblemDetails (400)
  v
Service Orchestration
  |
  | call Satlink.Logic services / Satlink.Infrastructure
  v
Result mapping
  |
  | success -> ApiResponse<T> (200)
  | failure -> ProblemDetails (4xx/5xx)
  v
Client
```

## Patrones y decisiones

### Controller thin / orchestration-only
Los controladores no implementan reglas de negocio. Se limita a:
- Validar request.
- Llamar a servicios.
- Mapear resultados a HTTP.

### Validación
- DTO validation con FluentValidation.
- Respuesta uniforme con `ValidationProblemDetails` (RFC 7807).

### Manejo de errores
- Para errores esperados: `ProblemDetails` con status apropiado.
- Para errores inesperados: `500` con logging estructurado.

### Logging
- `ILogger<T>`
- Mensajes con placeholders para logging estructurado.
- No se registra información sensible (p.ej. passwords).

### Autenticación JWT
- Access token (1h) con claims: `sub`, `email`, `role`.
- Refresh tokens persistidos en DB (`RefreshTokens`).
- Revocación del refresh token usado en refresh.

### Persistencia (EF Core)
- `AemetDbContext` incluye ahora:
  - `UserAccounts`
  - `RefreshTokens`
- Índice unique para `UserAccount.Email`.

## Extensiones futuras

- Añadir controllers CRUD completos cuando existan services de dominio para Create/Update/Delete.
- Normalizar modelos `Request` a PascalCase y mapear serialización según convención.
- Introducir excepciones específicas (`RequestNotFoundException`, etc.) con middleware centralizado.
