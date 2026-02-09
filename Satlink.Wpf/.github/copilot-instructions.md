# Copilot instructions

Follow the master rules defined in `.github/instructions/rules.instructions.md`.

## Style and conventions
- Allman style: braces on a new line.
- Interfaces are prefixed with `I`.
- Services are suffixed with `Service`.
- `async` methods are suffixed with `Async`.
- Private fields use `_camelCase`.
- Avoid `var` unless the type is obvious.

## API
- Do not include business logic in controllers.
- Use `Result<T>` or `ApiResponse<T>` for responses.
- Use RFC 7807 (`ProblemDetails`) for errors.
- Use structured logging with `ILogger`.

## Validation
- Implement input validation with FluentValidation.

## Documentation
- XML docs are required for public methods.
- Use inline comments for key logic steps.

## Testing
- xUnit + NSubstitute.
- Arrange-Act-Assert.
- Test names: `MethodUnderTest_Scenario_ExpectedResult`.
