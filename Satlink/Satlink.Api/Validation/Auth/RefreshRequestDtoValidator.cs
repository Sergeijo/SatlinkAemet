using FluentValidation;

using Satlink.Api.Dtos.Auth;

namespace Satlink.Api.Validation.Auth;

/// <summary>
/// Validates <see cref="RefreshRequestDto"/>.
/// </summary>
public sealed class RefreshRequestDtoValidator : AbstractValidator<RefreshRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshRequestDtoValidator"/> class.
    /// </summary>
    public RefreshRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
