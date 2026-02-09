# Project master rules

These rules apply to all code in this workspace.

## Code conventions

- Interfaces must be prefixed with `I`.
- Services must be suffixed with `Service`.
- `async` methods must be suffixed with `Async`.
- Use `var` only when the type is obvious.
- Allman brace style (opening brace on a new line).
- Private fields use `_camelCase`.

## Documentation

- XML comments are required on all public methods:
  - `/// <summary>`
  - `/// <param>`
  - `/// <returns>`
  - `/// <exception>` when applicable.
- Use inline comments to describe relevant logic steps.

## API

- Responses must be wrapped in `Result<T>` or `ApiResponse<T>`.
- Errors must follow RFC 7807 (`ProblemDetails`).
- Use structured logging with `ILogger`.
- `Console.WriteLine` is forbidden in production.

## Validation

- Input DTO validation must be implemented with FluentValidation.

## Testing

- Tests use xUnit and NSubstitute.
- Arrange-Act-Assert pattern.
- Test names: `MethodUnderTest_Scenario_ExpectedResult`.
