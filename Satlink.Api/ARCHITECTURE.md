# Satlink.Api - Architecture

## Goal

`Satlink.Api` implements the **Presentation** layer of the solution:
- Translates HTTP/JSON into DTOs.
- Validates input.
- Orchestrates calls to `Satlink.Logic`.
- Returns uniform responses (`ApiResponse<T>` and `ProblemDetails`).

## Modules

- `Controllers/`
  - REST endpoints.
  - No business logic.

- `Dtos/`
  - Input/output contracts for HTTP.

- `Validation/`
  - FluentValidation validators.

- `Services/`
  - API-specific services (e.g., `TokenService`) that are not business rules.

- `Contracts/`
  - `ApiResponse<T>`, `PagedResult<T>`, RFC 7807 helpers.

- `Utilities/`
  - `PaginationHelper`.

## Request flow (diagram)

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

## Patterns and decisions

### Thin controllers / orchestration-only
Controllers do not implement business rules. They are limited to:
- Validating the request.
- Calling services.
- Mapping results to HTTP.

### Validation
- DTO validation with FluentValidation.
- Uniform response with `ValidationProblemDetails` (RFC 7807).

### Error handling
- For expected errors: `ProblemDetails` with the appropriate status.
- For unexpected errors: `500` with structured logging.

### Logging
- `ILogger<T>`
- Messages use placeholders for structured logging.
- Do not log sensitive information (e.g., passwords).

### JWT authentication
- Access token (1h) with claims: `sub`, `email`, `role`.
- Refresh tokens persisted in DB (`RefreshTokens`).
- Revoke the refresh token used during refresh.

### Persistence (EF Core)
- `AemetDbContext` currently includes:
  - `UserAccounts`
  - `RefreshTokens`
- Unique index for `UserAccount.Email`.

## Future extensions

- Add full CRUD controllers once domain services exist for Create/Update/Delete.
- Normalize `Request` models to PascalCase and map serialization based on convention.
- Introduce specific exceptions (`RequestNotFoundException`, etc.) with centralized middleware.
