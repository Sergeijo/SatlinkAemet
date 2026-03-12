using FluentValidation;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Api.Validation.Aemet;

/// <summary>
/// Validates the <see cref="MarineZonePredictionDto"/> body used on the
/// AEMET downloads update endpoint.
/// </summary>
public sealed class UpdateAemetDownloadBodyValidator : AbstractValidator<MarineZonePredictionDto>
{
    public UpdateAemetDownloadBodyValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty()
            .WithMessage("Zone id must not be empty.");

        RuleFor(x => x.nombre)
            .NotEmpty()
            .WithMessage("Zone name must not be empty.");

        RuleFor(x => x.origen)
            .NotNull()
            .WithMessage("Origen must not be null.");

        RuleFor(x => x.prediccion)
            .NotNull()
            .WithMessage("Prediccion must not be null.");
    }
}
