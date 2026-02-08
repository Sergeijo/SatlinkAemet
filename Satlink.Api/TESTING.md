# Satlink.Api - Testing

## Objetivo

Los tests unitarios de `Satlink.Api` validan la lógica de orquestación de los controladores:
- Mapeo de resultados a HTTP.
- Manejo de errores.
- Validaciones esperadas.

## Tecnologías

- xUnit
- NSubstitute
- EF Core InMemory (para `AuthController`/`AemetDbContext`)

## Proyecto de tests

- `Satlink.Api.Tests`

## Convenciones

- Arrange-Act-Assert.
- Nombres descriptivos: `MetodoTesteado_Escenario_ResultadoEsperado`.
- No usar servicios reales externos.
- No incluir secretos.

## Ejecutar tests (local/CI)

### Visual Studio
- Build solution.
- Test Explorer -> Run All.

### CLI

```bash
# desde la raíz del repo/solution
# (requiere restore NuGet habilitado)
dotnet test .\Satlink.Api.Tests\Satlink.Api.Tests.csproj -c Release
```

## Qué se testea

### `AuthController`
- `LoginAsync`
  - Happy path: credenciales válidas -> `200 OK` con tokens.
  - Error: credenciales inválidas -> `401 Unauthorized`.
- `RefreshAsync`
  - Happy path: refresh válido -> `200 OK`.
  - Error: refresh inválido -> `401 Unauthorized`.

### `AemetValuesController`
- `GetValues`
  - Happy path -> `200 OK`.
  - Error: `Result.Fail` -> `400 Bad Request`.
  - Error: excepción inesperada -> `500 Internal Server Error`.

## Troubleshooting

### Fallos de restore NuGet
- Verifica conectividad a nuget.org.
- Revisa `NuGet.config` y proxies.
- Limpia caches:

```bash
dotnet nuget locals all --clear
```

### InMemory DB
- Cada test usa un nombre de base de datos distinto (`Guid.NewGuid().ToString()`) para evitar contaminación entre tests.
