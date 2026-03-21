using FluentValidation;

using Satlink.Logic.CQRS.AemetValues.Commands;

namespace Satlink.Logic.CQRS.AemetValues.Validators;

/// <summary>
/// Validates <see cref="SaveAemetDownloadsCommand"/> before the handler executes.
/// </summary>
public sealed class SaveAemetDownloadsCommandValidator : AbstractValidator<SaveAemetDownloadsCommand>
{
    public SaveAemetDownloadsCommandValidator()
    {
        RuleFor(x => x.Predictions)
            .NotNull()
                .WithMessage("La lista de predicciones no puede ser nula.")
            .NotEmpty()
                .WithMessage("Debe incluirse al menos una predicción.");
    }
}
