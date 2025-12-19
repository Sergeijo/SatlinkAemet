# Satlink.Api - API

Este documento describe los endpoints expuestos por `Satlink.Api`.

## Convenciones de respuesta

### Éxito
- Respuesta: `ApiResponse<T>`

Ejemplo:
- `200 OK`

```json
{
  "data": { }
}
```

### Error (RFC 7807)
- Respuesta: `ProblemDetails` / `ValidationProblemDetails`

Ejemplo:
- `400 Bad Request`

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "...",
  "instance": "/api/..."
}
```

## Autenticación

- Esquema: `Bearer` (JWT)
- Header:

```
Authorization: Bearer <accessToken>
```

## Endpoints

## POST /api/auth/login

Autentica un usuario y retorna tokens.

### Request

Body: `LoginDto`

```json
{
  "email": "user@test.com",
  "password": "password123"
}
```

### Responses

- `200 OK` -> `ApiResponse<AuthResponse>`
- `401 Unauthorized` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

### Response (200)

```json
{
  "data": {
    "accessToken": "<jwt>",
    "refreshToken": "<refresh>",
    "expiresIn": 3600,
    "user": {
      "id": "00000000-0000-0000-0000-000000000000",
      "email": "user@test.com",
      "role": "User"
    }
  }
}
```

## POST /api/auth/refresh

Renueva el access token usando un refresh token almacenado.

### Request

Body: `RefreshRequestDto`

```json
{
  "refreshToken": "<refresh>"
}
```

### Responses

- `200 OK` -> `ApiResponse<AuthResponse>`
- `401 Unauthorized` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

### Notes

- Si el refresh token está revocado/expirado/no existe -> `401 Unauthorized`.

## POST /api/aemetvalues/values

Obtiene predicciones AEMET para una zona.

### Request

Body: `GetAemetValuesRequestDto`

```json
{
  "apiKey": "<api_key>",
  "url": "https://opendata.aemet.es/opendata/api/prediccion/maritima",
  "zone": 1
}
```

### Responses

- `200 OK` -> `ApiResponse<List<Request>>`
- `400 Bad Request` -> `ProblemDetails` (incluye fallos de servicio o validación)
- `500 Internal Server Error` -> `ProblemDetails`

### Response (200)

```json
{
  "data": [
    {
      "id": "...",
      "nombre": "...",
      "origen": { },
      "situacion": { },
      "prediccion": { }
    }
  ]
}
```

## Requests (CRUD)

> Nota: el modelo actual `Request` usa `string id` en `Satlink.Domain`. Los endpoints CRUD enrutan por `{id:int}` y el acceso actual compara `id.ToString()`. Se recomienda normalizar el id a `int` o `Guid` en el dominio para coherencia completa.

## GET /api/requests

Retorna la lista completa de `Request`.

### Responses

- `200 OK` -> `ApiResponse<List<Request>>`
- `500 Internal Server Error` -> `ProblemDetails`

## GET /api/requests/{id}

Retorna un `Request` por id.

### Responses

- `200 OK` -> `ApiResponse<Request>`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## POST /api/requests

Crea un `Request`.

### Request

Body: `CreateRequestDto`

```json
{
  "nombre": "Zona 1"
}
```

### Responses

- `201 Created` -> `ApiResponse<Request>`
- `400 Bad Request` -> `ValidationProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## PUT /api/requests/{id}

Actualiza un `Request`.

### Request

Body: `UpdateRequestDto`

```json
{
  "nombre": "Zona 1 (actualizada)"
}
```

### Responses

- `200 OK` -> `ApiResponse<Request>`
- `400 Bad Request` -> `ValidationProblemDetails`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## DELETE /api/requests/{id}

Elimina un `Request`.

### Responses

- `204 No Content`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## Códigos de estado comunes

- `400` Validación / petición inválida
- `401` No autenticado / credenciales inválidas
- `403` Autenticado pero sin permisos
- `404` Recurso no encontrado (cuando el controlador lo implemente)
- `500` Error inesperado
