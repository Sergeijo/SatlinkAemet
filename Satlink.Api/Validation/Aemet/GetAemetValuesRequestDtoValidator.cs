using FluentValidation;

using Satlink.Api.Dtos.Aemet;

namespace Satlink.Api.Validation.Aemet;

/// <summary>
/// Validates <see cref="GetAemetValuesRequestDto"/>.
/// </summary>
public sealed class GetAemetValuesRequestDtoValidator : AbstractValidator<GetAemetValuesRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAemetValuesRequestDtoValidator"/> class.
    /// </summary>
    public GetAemetValuesRequestDtoValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty();

        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(BeValidAbsoluteUrl)
            .WithMessage("Url must be a valid absolute URL.");

        RuleFor(x => x.Zone)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeValidAbsoluteUrl(string url)
    {
        // Validate as absolute URL.
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
