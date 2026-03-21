# SatlinkAemet

Solution for the **Satlink technical test** based on Clean Architecture with:

- **WPF (MVVM)** client.
- **Angular** client built with CLI 19 (Standalone Components, Signals and Built-in Control Flow).
- **REST API** in **.NET 10** with CQRS, MediatR pipeline behaviours, MassTransit + RabbitMQ outbox, and JWT authentication.
- **Domain / Application (Logic) / Infrastructure / Contracts** layers.
- **Test** projects.

> Note: this repository originally targeted .NET 5; the solution is now aligned to **.NET 10**.

---

## Table of contents

1. [Repository structure](#repository-structure)
2. [Architecture overview](#architecture-overview)
3. [CQRS pipeline (MediatR behaviours)](#cqrs-pipeline-mediatR-behaviours)
4. [HTTP middleware pipeline](#http-middleware-pipeline)
5. [Messaging – RabbitMQ + MassTransit outbox](#messaging--rabbitmq--masstransit-outbox)
6. [Authentication](#authentication)
7. [Available endpoints](#available-endpoints)
8. [Getting started](#getting-started)
9. [Configuration reference](#configuration-reference)
10. [Project conventions](#project-conventions)
11. [Troubleshooting](#troubleshooting)

---

## Repository structure

```
SatlinkAemet/
├── Satlink.sln
├── Satlink.Api/            # Presentation layer (ASP.NET Core Web API)
├── Satlink.Logic/          # Application layer (CQRS, services, interfaces)
├── Satlink.Infrastructure/ # Infrastructure layer (EF Core, Dapper, MassTransit)
├── Satlink.Domain/         # Domain layer (entities, value objects)
├── Satlink.Contracts/      # Shared DTOs and contracts
├── Satlink.Wpf/            # WPF desktop client (MVVM)
├── Satlink.Angular/        # Angular web client
├── Satlink.Tests/          # Unit tests
├── Satlink.Api.Tests/      # Integration / legacy tests
├── docker-compose.yml      # RabbitMQ for local development
└── README.md
```

---

## Architecture overview

```
HTTP Request
     │
     ▼
┌─────────────────────────────────────────────┐
│  Satlink.Api  (Presentation)                        │
│  Controllers · Validators · Middlewares             │
│  GlobalExceptionMiddleware                          │
│  RequestLoggingMiddleware (+ IUserContext)          │
└───────────────────┬─────────────────────────┘
                        │  MediatR (IRequest / ICommand)
                        ▼
┌─────────────────────────────────────────────┐
│  Satlink.Logic  (Application)                       │
│  CQRS pipeline behaviours:                          │
│    ExceptionBehaviour                               │
│    LoggingBehaviour  ◄── IUserContext              │
│    ValidationBehaviour (FluentValidation)           │
│    TransactionBehaviour (ITransactionalCmd)         │
│  Services · Interfaces · Integration events.        │
└──────────┬────────────────┬─────────────────┘
             │ EF Core / SQL     │ IEventBus
             ▼                  ▼
┌──────────────────┐  ┌────────────────────────┐
│ Satlink.Infra       │  │ MassTransit                 │
│ AemetDbContext      │  │ RabbitMQ transport          │
│ (SQL Server)        │  │ EF Core outbox              │
│ AemetSqliteCtx      │  │ Consumers                   │
│ Dapper repos        │  └────────────────────────┘
└──────────────────┘
           │
           ▼
┌─────────────────────────────────────────────┐
│  Satlink.Domain                                     │
│  PersistedRequest · Origen · Situacion              │
│  Prediccion · Zona                                  │
└─────────────────────────────────────────────┘
```

### Databases

| Store | Technology | Purpose |
|---|---|---|
| `SatlinkAemet` (SQL Server / LocalDB) | EF Core | Requests, MassTransit outbox tables |
| `aemet_downloads.db` (SQLite) | EF Core + Dapper | AEMET download history (read/write side) |

---

## CQRS pipeline (MediatR behaviours)

Behaviours are executed in the following order for every command and query:

```
Request
  │
  ▼  1. ExceptionBehaviour
  │     Catches any unhandled exception, logs it, and re-throws.
  │     ValidationException is re-thrown silently (handled downstream).
  │
  ▼  2. LoggingBehaviour
  │     Logs request start/end with elapsed time.
  │     Records the identity of the user executing the request (via IUserContext).
  │
  ▼  3. ValidationBehaviour
  │     Runs all FluentValidation IValidator<TRequest> implementations.
  │     Throws ValidationException on the first set of failures.
  │
  ▼  4. TransactionBehaviour
  │     Only active for commands that implement ITransactionalCommand.
  │     Wraps the handler in a SQL Server transaction (via IUnitOfWork).
  │     Rolls back on exception OR when the Result indicates failure.
  │
  ▼  Handler
```

### Transactional commands

The following commands are wrapped in a database transaction automatically:

| Command | Description |
|---|---|
| `CreateRequestCommand` | Creates a request and publishes `RequestCreatedIntegrationEvent` |
| `UpdateRequestCommand` | Updates an existing request |
| `DeleteRequestCommand` | Deletes a request |

### FluentValidation validators

| Validator | Guards |
|---|---|
| `CreateRequestCommandValidator` | Required `Nombre` field |
| `UpdateRequestCommandValidator` | Required `Nombre` field |
| `SaveAemetDownloadsCommandValidator` | Non-null, non-empty prediction list |

---

## HTTP middleware pipeline

```
Incoming HTTP request
        │
        ▼
GlobalExceptionMiddleware   ← catches ValidationException → 400, Exception → 500
        │                      returns RFC 7807 ProblemDetails / ValidationProblemDetails
        ▼
UseRouting / UseAuthentication (JWT Bearer)
        │
        ▼
RequestLoggingMiddleware    ← logs method, path, status code, elapsed time
        │                      populates IUserContext from JWT claims for the request scope
        ▼
UseAuthorization
        │
        ▼
Controllers
```

### IUserContext

`IUserContext` is a scoped abstraction that exposes the authenticated user's identity to any service or behaviour without a direct dependency on `IHttpContextAccessor`.

| Property | Source |
|---|---|
| `UserId` | `ClaimTypes.NameIdentifier` |
| `Email` | `ClaimTypes.Email` |
| `Role` | `ClaimTypes.Role` |
| `IsAuthenticated` | `HttpContext.User.Identity.IsAuthenticated` |

---

## Messaging – RabbitMQ + MassTransit outbox

### Integration events

| Event | Published by | Consumed by |
|---|---|---|
| `RequestCreatedIntegrationEvent` | `CreateRequestCommandHandler` (on success) | `RequestCreatedConsumer` |
| `AemetDownloadSavedIntegrationEvent` | `SaveAemetDownloadsCommandHandler` (when rows saved > 0) | `AemetDownloadSavedConsumer` |

### Outbox pattern

Integration events are **not published directly to RabbitMQ**. They are stored atomically in SQL Server as part of the same EF Core transaction (using the MassTransit EF Core outbox). The MassTransit background service then delivers them to RabbitMQ, guaranteeing at-least-once delivery even if the broker is temporarily unavailable.

```
Handler
  │  publishes via IEventBus (IPublishEndpoint)
  │
  ▼
SQL Server (AemetDbContext)
  ├── OutboxMessage   ← integration event stored here
  ├── OutboxState
  └── InboxState
  │
  ▼ (MassTransit background worker)
RabbitMQ → Consumer
```

### Starting RabbitMQ locally

```bash
docker compose up -d
```

Management UI: [http://localhost:15672](http://localhost:15672) (guest / guest)

---

## Authentication

Endpoints (except login/refresh) require a valid **JWT Bearer** token.

```
Authorization: Bearer <token>
```

Obtain a token via `POST /api/auth/login`.

---

## Available endpoints

### Auth

| Method | Path | Auth |
|---|---|---|
| `POST` | `/api/auth/login` | ❌ |
| `POST` | `/api/auth/refresh` | ❌ |

### AEMET

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/aemetvalues/values` | ✅ | Fetch marine zone predictions from AEMET API and persist new results |

### Requests (CRUD)

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/requests` | ✅ | List all requests |
| `GET` | `/api/requests/{id}` | ✅ | Get a request by ID |
| `POST` | `/api/requests` | ✅ | Create a request |
| `PUT` | `/api/requests/{id}` | ✅ | Update a request |
| `DELETE` | `/api/requests/{id}` | ✅ | Delete a request |

---

## Getting started

### Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 10.x |
| SQL Server / LocalDB | Any recent version |
| Docker (for RabbitMQ) | 20+ |
| Node.js + npm | (Angular client only) |

### 1 – Clone the repository

```bash
git clone https://github.com/Sergeijo/SatlinkAemet.git
cd SatlinkAemet
```

### 2 – Start RabbitMQ

```bash
docker compose up -d
```

### 3 – Configure the API

Edit `Satlink.Api/appsettings.json` (or use user secrets / environment variables):

```json
{
  "Jwt": {
    "Issuer": "Satlink",
    "Audience": "Satlink",
    "Key": "<min-32-char-secret-key>"
  },
  "ConnectionStrings": {
    "SatlinkApp": "Server=(localdb)\\MSSQLLocalDB;Database=SatlinkAemet;Trusted_Connection=True;TrustServerCertificate=True",
    "AemetDownloads": "Data Source=aemet_downloads.db"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### 4 – Run the API

```bash
dotnet run --project Satlink.Api/Satlink.Api.csproj
```

The SQL Server database (`SatlinkAemet`) is created automatically on first run via `EnsureCreatedAsync`. The SQLite database (`aemet_downloads.db`) is also initialised on startup.

Browse to `https://localhost:<port>/swagger`.

### 5 – Run the WPF client (Windows only)

```bash
dotnet run --project Satlink.Wpf/Satlink.Wpf.csproj
```

### 6 – Run the Angular client

```bash
cd Satlink.Angular
npm install
npm start
```

---

## Configuration reference

| Key | Description |
|---|---|
| `Jwt:Issuer` | JWT issuer claim |
| `Jwt:Audience` | JWT audience claim |
| `Jwt:Key` | Signing key (min 32 chars; store in Secret Manager / CI secret) |
| `ConnectionStrings:SatlinkApp` | SQL Server connection string (requests + outbox tables) |
| `ConnectionStrings:AemetDownloads` | SQLite connection string (AEMET download history) |
| `RabbitMQ:Host` | RabbitMQ hostname |
| `RabbitMQ:VirtualHost` | RabbitMQ virtual host |
| `RabbitMQ:Username` | RabbitMQ username |
| `RabbitMQ:Password` | RabbitMQ password |

---

## Project conventions

Full rules are defined in `.github/instructions/rules.instructions.md`. Summary:

- Interfaces prefixed with `I` (e.g., `IEventBus`).
- Services suffixed with `Service` (e.g., `AemetValuesService`).
- `async` methods suffixed with `Async`.
- Allman brace style.
- `ILogger` for all logging — no `Console.WriteLine`.
- XML documentation on all public members.
- Commands implement `ITransactionalCommand` to opt in to the transaction behaviour.
- Integration events are plain records suffixed with `IntegrationEvent`.

---

## Troubleshooting

### `Cannot open database "SatlinkAemet"`

The SQL Server database does not exist yet. The application creates it automatically on startup via `EnsureCreatedAsync`. Ensure LocalDB (or SQL Server) is running and the connection string in `appsettings.json` is correct.

### `BrokerUnreachableException` on startup

RabbitMQ is not running. Start it with:

```bash
docker compose up -d
```

MassTransit will retry the connection automatically every 30 seconds once the broker is available.

### `401 Unauthorized` on protected endpoints

- Include the header: `Authorization: Bearer <token>`.
- Verify `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:Key` match the values used to issue the token.

### `400 Bad Request` with validation errors

FluentValidation failures are returned as `ValidationProblemDetails` (RFC 7807) with per-field error details:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nombre": ["'Nombre' must not be empty."]
  }
}
```

### Invalid or expired refresh token

The token may be revoked, expired, or absent from the database. Re-authenticate via `POST /api/auth/login`.
