using System.Collections.Generic;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.ApiClient;

/// <summary>
/// Represents the AEMET values retrieval result for the WPF client.
/// </summary>
public sealed class AemetValuesResult
{
    private AemetValuesResult(bool success, string? error, List<MarineZonePredictionDto>? value)
    {
        Success = success;
        Error = error ?? string.Empty;
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Gets the retrieved value.
    /// </summary>
    public List<MarineZonePredictionDto>? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The retrieved items.</param>
    /// <returns>The created result.</returns>
    public static AemetValuesResult Ok(List<MarineZonePredictionDto> value)
    {
        return new AemetValuesResult(true, null, value);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>The created result.</returns>
    public static AemetValuesResult Fail(string error)
    {
        return new AemetValuesResult(false, error, null);
    }
}
