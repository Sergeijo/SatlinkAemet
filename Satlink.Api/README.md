# Satlink.Api

API REST en **.NET 10** para exponer los servicios del proyecto `Satlink.Logic` y funcionalidades transversales (autenticación JWT, validación, paginación).

## Objetivos

- Actuar como capa **Presentation**:
  - Orquesta peticiones HTTP hacia `Satlink.Logic`.
  - No contiene lógica de negocio.
  - Respuestas uniformes con `ApiResponse<T>` y errores RFC 7807 (`ProblemDetails`).
- Exponer endpoints autenticados mediante JWT (`/api/auth/login`, `/api/auth/refresh`).

## Arquitectura (alto nivel)

- `Satlink.Api` (Presentation)
  - Controladores (`ControllerBase`) con attribute routing.
  - DTOs + FluentValidation para validación de entrada.
  - Logging estructurado con `ILogger`.
  - Swagger/OpenAPI con esquema Bearer.

- `Satlink.Logic` (Application)
  - Servicios `*Service` consumidos desde la API.

- `Satlink.Infrastructure` (Data)
  - EF Core `AemetDbContext`.
  - Repositorios.

- `Satlink.Domain` (Domain)
  - Entidades de dominio.

## Convenciones del proyecto

Las reglas maestras están en:
- `.github/instructions/rules.instructions.md`

Resumen:
- Interfaces con prefijo `I`.
- Servicios con sufijo `Service`.
- Métodos `async` con sufijo `Async`.
- Allman style (llaves en nueva línea).
- `ILogger` para logging (no `Console.WriteLine`).
- XML docs en métodos públicos.

## Endpoints disponibles

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

## Desarrollo local

### Requisitos
- .NET SDK 10
- SQL Server / LocalDB (según `ConnectionStrings:SatlinkApp`)

### Configuración

Archivo: `Satlink.Api/appsettings.json`

- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key` (mínimo recomendado: 32 caracteres, gestionar por Secret Manager/CI)
- `ConnectionStrings:SatlinkApp`

### Ejecutar

- Ejecutar `Satlink.Api` desde Visual Studio.
- Navegar a `/swagger`.

## Troubleshooting

### 401 Unauthorized al llamar endpoints protegidos
- Verifica que envías header: `Authorization: Bearer <token>`.
- Revisa que `Jwt:Issuer`, `Jwt:Audience` y `Jwt:Key` coinciden con los usados para generar el token.

### Refresh token inválido
- El refresh token puede estar revocado, expirado o no existir en base de datos.

### Errores de validación de DTO (400)
- La API devuelve `ValidationProblemDetails` (RFC 7807) con el detalle por campo.
