# Satlink.Api - Testing

## Goal

The unit tests for `Satlink.Api` validate the controllers' orchestration logic:
- Mapping results to HTTP.
- Error handling.
- Expected validations.

## Technologies

- xUnit
- NSubstitute
- EF Core InMemory (for `AuthController`/`AemetDbContext`)

## Test project

- `Satlink.Api.Tests`

## Conventions

- Arrange-Act-Assert.
- Descriptive names: `MethodUnderTest_Scenario_ExpectedResult`.
- Do not use real external services.
- Do not include secrets.

## Running tests (local/CI)

### Visual Studio
- Build the solution.
- Test Explorer -> Run All.

### CLI

```bash
# from the repo/solution root
# (requires NuGet restore to be enabled)
dotnet test .\Satlink.Api.Tests\Satlink.Api.Tests.csproj -c Release
```

## What is tested

### `AuthController`
- `LoginAsync`
  - Happy path: valid credentials -> `200 OK` with tokens.
  - Error: invalid credentials -> `401 Unauthorized`.
- `RefreshAsync`
  - Happy path: valid refresh -> `200 OK`.
  - Error: invalid refresh -> `401 Unauthorized`.

### `AemetValuesController`
- `GetValues`
  - Happy path -> `200 OK`.
  - Error: `Result.Fail` -> `400 Bad Request`.
  - Error: unexpected exception -> `500 Internal Server Error`.

## Troubleshooting

### NuGet restore failures
- Verify connectivity to nuget.org.
- Check `NuGet.config` and proxies.
- Clear caches:

```bash
dotnet nuget locals all --clear
```

### InMemory DB
- Each test uses a different database name (`Guid.NewGuid().ToString()`) to avoid cross-test contamination.
