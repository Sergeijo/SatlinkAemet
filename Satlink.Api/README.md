# Satlink.Api

REST API in **.NET 10** to expose the services from `Satlink.Logic` and cross-cutting features (JWT authentication, validation, pagination).

## Goals

- Act as the **Presentation** layer:
  - Orchestrates HTTP requests into `Satlink.Logic`.
  - Contains no business logic.
  - Uniform responses via `ApiResponse<T>` and RFC 7807 errors (`ProblemDetails`).
- Expose JWT-protected endpoints (`/api/auth/login`, `/api/auth/refresh`).

## Architecture (high level)

- `Satlink.Api` (Presentation)
  - Controllers (`ControllerBase`) with attribute routing.
  - DTOs + FluentValidation for input validation.
  - Structured logging with `ILogger`.
  - Swagger/OpenAPI with Bearer scheme.

- `Satlink.Logic` (Application)
  - `*Service` services consumed by the API.

- `Satlink.Infrastructure` (Data)
  - EF Core `AemetDbContext`.
  - Repositories.

- `Satlink.Domain` (Domain)
  - Domain entities.

## Project conventions

The master rules are in:
- `.github/instructions/rules.instructions.md`

Summary:
- Interfaces are prefixed with `I`.
- Services are suffixed with `Service`.
- `async` methods are suffixed with `Async`.
- Allman style (braces on a new line).
- Use `ILogger` for logging (no `Console.WriteLine`).
- XML docs on public methods.

## Available endpoints

### Auth
- `POST /api/auth/login`
- `POST /api/auth/refresh`

### AEMET
- `POST /api/aemetvalues/values`

### Requests (CRUD)
- `GET /api/requests`
- `GET /api/requests/{id}`
- `POST /api/requests`
- `PUT /api/requests/{id}`
- `DELETE /api/requests/{id}`

## Local development

### Requirements
- .NET SDK 10
- SQL Server / LocalDB (depending on `ConnectionStrings:SatlinkApp`)

### Configuration

File: `Satlink.Api/appsettings.json`

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key` (recommended minimum: 32 characters; manage with Secret Manager/CI)
- `ConnectionStrings:SatlinkApp`

### Run

- Run `Satlink.Api` from Visual Studio.
- Browse to `/swagger`.

## Troubleshooting

### 401 Unauthorized when calling protected endpoints
- Make sure you send the header: `Authorization: Bearer <token>`.
- Verify `Jwt:Issuer`, `Jwt:Audience` and `Jwt:Key` match the ones used to issue the token.

### Invalid refresh token
- The refresh token may be revoked, expired, or missing from the database.

### DTO validation errors (400)
- The API returns `ValidationProblemDetails` (RFC 7807) with field-level details.
