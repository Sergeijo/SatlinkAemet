# SatlinkAemet

Solution for the **Satlink technical test** based on a Clean-Architecture with:

- **WPF (MVVM)** client.
- **Angular** client built with CLI 19 (with Standalone Components, Signals and Built-in Control Flow).
- **REST API** in **.NET 10**.
- **Domain / Application (Logic) / Infrastructure / Contracts** layers.
- **Test** projects.

> Note: this repository originally targeted .NET 5; the solution is now aligned to **.NET 10**.

## Repository structure

- `Satlink.sln`: main solution.
- `Satlink.Wpf`: WPF desktop app (MVVM pattern).
- `Satlink.Angular`: web front-end (Angular).
- `Satlink.Api`: REST API (Swagger, JWT, validation, uniform responses).
- `Satlink.Logic`: application layer (the `*Service` services consumed by API/WPF).
- `Satlink.Domain`: domain entities and domain logic.
- `Satlink.Infrastructure`: data access (EF Core, `AemetDbContext`, repositories).
- `Satlink.Contracts`: shared contracts/DTOs between layers.
- `Satlink.Tests`: tests (logic/WPF as applicable).
- `Satlink.Api.Tests`: API tests.

## Architecture (high level)

- Presentation:
  - `Satlink.Wpf`
  - `Satlink.Angular`
  - `Satlink.Api`
- Application:
  - `Satlink.Logic`
- Domain:
  - `Satlink.Domain`
- Infrastructure:
  - `Satlink.Infrastructure`
- Shared contracts:
  - `Satlink.Contracts`

## Requirements

- .NET SDK **10**
- (For `Satlink.Wpf`) Windows with `Microsoft.WindowsDesktop.App`
- (For `Satlink.Api`) SQL Server / LocalDB depending on `ConnectionStrings:SatlinkApp`
- (For `Satlink.Angular`) Node.js + npm

## How to run

### Visual Studio

1. Open `Satlink.sln`.
2. Select the startup project:
   - `Satlink.Wpf` for desktop.
   - `Satlink.Api` for API.
3. Build and run.

### CLI (.NET)

- Build:
  - `dotnet build Satlink.sln`

- Run API:
  - `dotnet run --project Satlink.Api/Satlink.Api.csproj`
  - Browse to `https://localhost:<port>/swagger`

- Run WPF:
  - `dotnet run --project Satlink.Wpf/Satlink.Wpf.csproj`

### Angular

From `Satlink.Angular`:

- `npm install`
- `npm start` (or `ng serve`)

## Additional documentation

- API: see `Satlink.Api/README.md`.
- Repository conventions: `.github/instructions/rules.instructions.md`.
