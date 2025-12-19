using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Satlink.Api.Contracts;

/// <summary>
/// Provides helpers for creating RFC 7807 <see cref="ProblemDetails"/> responses.
/// </summary>
public static class ProblemDetailsFactoryExtensions
{
    /// <summary>
    /// Creates a <see cref="ProblemDetails"/> instance.
    /// </summary>
    /// <param name="httpContext">The current http context.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="title">A short, human-readable summary of the problem type.</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    /// <param name="type">A URI reference that identifies the problem type.</param>
    /// <returns>The created problem details.</returns>
    public static ProblemDetails CreateProblemDetails(
        this HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        string? type = null)
    {
        // Create standard RFC 7807 payload.
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = httpContext.Request.Path
        };
    }
}
