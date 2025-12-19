using FluentValidation;

using Satlink.Api.Dtos.Requests;

namespace Satlink.Api.Validation.Requests;

/// <summary>
/// Validates <see cref="UpdateRequestDto"/>.
/// </summary>
public sealed class UpdateRequestDtoValidator : AbstractValidator<UpdateRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateRequestDtoValidator"/> class.
    /// </summary>
    public UpdateRequestDtoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(200);
    }
}
