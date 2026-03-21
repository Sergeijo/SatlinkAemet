using FluentValidation;

using Satlink.Logic.CQRS.Requests.Commands;

namespace Satlink.Logic.CQRS.Requests.Validators;

/// <summary>
/// Validates <see cref="CreateRequestCommand"/> before the handler executes.
/// </summary>
public sealed class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
                .WithMessage("El nombre es obligatorio.")
            .MaximumLength(200)
                .WithMessage("El nombre no puede superar los 200 caracteres.");
    }
}
