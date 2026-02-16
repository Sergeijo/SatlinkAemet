# Satlink.Api - API

This document describes the endpoints exposed by `Satlink.Api`.

## Response conventions

### Success
- Response: `ApiResponse<T>`

Example:
- `200 OK`

```json
{
  "data": { }
}
```

### Error (RFC 7807)
- Response: `ProblemDetails` / `ValidationProblemDetails`

Example:
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

## Authentication

- Scheme: `Bearer` (JWT)
- Header:

```
Authorization: Bearer <accessToken>
```

## Endpoints

## POST /api/auth/login

Authenticates a user and returns tokens.

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

Renews the access token using a stored refresh token.

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

- If the refresh token is revoked/expired/missing -> `401 Unauthorized`.

## POST /api/aemetvalues/values

Gets AEMET forecasts for a zone.

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

- `200 OK` -> `ApiResponse<List<RequestDto>>`
- `400 Bad Request` -> `ProblemDetails` (includes service failures or validation errors)
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

> Note: the current `Request` model uses `string id` in `Satlink.Domain`. The CRUD endpoints route using `{id:int}` and the current access compares `id.ToString()`. Consider normalizing the id to `int` or `Guid` in the domain for full consistency.

## GET /api/requests

Returns the full list of `Request`.

### Responses

- `200 OK` -> `ApiResponse<List<Request>>`
- `500 Internal Server Error` -> `ProblemDetails`

## GET /api/requests/{id}

Returns a `Request` by id.

### Responses

- `200 OK` -> `ApiResponse<RequestDto>`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## POST /api/requests

Creates a `Request`.

### Request

Body: `CreateRequestDto`

```json
{
  "nombre": "Zona 1"
}
```

### Responses

- `201 Created` -> `ApiResponse<RequestDto>`
- `400 Bad Request` -> `ValidationProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## PUT /api/requests/{id}

Updates a `Request`.

### Request

Body: `UpdateRequestDto`

```json
{
  "nombre": "Zona 1 (actualizada)"
}
```

### Responses

- `200 OK` -> `ApiResponse<RequestDto>`
- `400 Bad Request` -> `ValidationProblemDetails`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## DELETE /api/requests/{id}

Deletes a `Request`.

### Responses

- `204 No Content`
- `404 Not Found` -> `ProblemDetails`
- `500 Internal Server Error` -> `ProblemDetails`

## Common status codes

- `400` Validation / invalid request
- `401` Not authenticated / invalid credentials
- `403` Authenticated but not authorized
- `404` Resource not found (when implemented by the controller)
- `500` Unexpected error
