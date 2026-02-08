using FluentValidation;

using Satlink.Api.Dtos.Requests;

namespace Satlink.Api.Validation.Requests;

/// <summary>
/// Validates <see cref="CreateRequestDto"/>.
/// </summary>
public sealed class CreateRequestDtoValidator : AbstractValidator<CreateRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateRequestDtoValidator"/> class.
    /// </summary>
    public CreateRequestDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(200);
    }
}
