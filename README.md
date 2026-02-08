# SatlinkAemet

Solución para el **test técnico de Satlink** basada en una arquitectura por capas con:

- Cliente **WPF (MVVM)**.
- **API REST** en **.NET 10**.
- Cliente **Angular**.
- Capas de **Domain / Application (Logic) / Infrastructure / Contracts**.
- Proyectos de **tests**.

> Nota: este repositorio originalmente estaba en .NET 5; actualmente la solución está orientada a **.NET 10**.

## Estructura del repositorio

- `Satlink.sln`: solución principal.
- `Satlink.Wpf`: aplicación de escritorio WPF (patrón MVVM).
- `Satlink.Api`: API REST (Swagger, JWT, validación, respuestas uniformes).
- `Satlink.Angular`: front-end web (Angular).
- `Satlink.Logic`: capa de aplicación (servicios `*Service` consumidos por la API/WPF).
- `Satlink.Domain`: entidades y lógica de dominio.
- `Satlink.Infrastructure`: acceso a datos (EF Core, `AemetDbContext`, repositorios).
- `Satlink.Contracts`: contratos/DTOs compartidos entre capas.
- `Satlink.Tests`: tests (lógica/WPF según corresponda).
- `Satlink.Api.Tests`: tests de la API.

## Arquitectura (alto nivel)

- Presentation:
  - `Satlink.Wpf`
  - `Satlink.Api`
- Application:
  - `Satlink.Logic`
- Domain:
  - `Satlink.Domain`
- Infrastructure:
  - `Satlink.Infrastructure`
- Shared contracts:
  - `Satlink.Contracts`

## Requisitos

- .NET SDK **10**
- (Para `Satlink.Wpf`) Windows con `Microsoft.WindowsDesktop.App`
- (Para `Satlink.Api`) SQL Server / LocalDB según `ConnectionStrings:SatlinkApp`
- (Para `Satlink.Angular`) Node.js + npm

## Cómo ejecutar

### Visual Studio

1. Abrir `Satlink.sln`.
2. Seleccionar proyecto de inicio:
   - `Satlink.Wpf` para escritorio.
   - `Satlink.Api` para API.
3. Compilar y ejecutar.

### CLI (.NET)

- Compilar:
  - `dotnet build Satlink.sln`

- Ejecutar API:
  - `dotnet run --project Satlink.Api/Satlink.Api.csproj`
  - Navegar a `https://localhost:<puerto>/swagger`

- Ejecutar WPF:
  - `dotnet run --project Satlink.Wpf/Satlink.Wpf.csproj`

### Angular

Desde `Satlink.Angular`:

- `npm install`
- `npm start` (o `ng serve`)

## Documentación específica

- API: ver `Satlink.Api/README.md`.
- Convenciones del repositorio: `.github/instructions/rules.instructions.md`.
