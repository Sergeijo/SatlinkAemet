using System;
using System.Threading.Tasks;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Satlink.Api.Contracts;

namespace Satlink.Api.Middleware;

/// <summary>
/// First middleware in the pipeline.
/// Catches all unhandled exceptions and converts them to RFC 7807 Problem Details
/// responses, keeping the controllers free of try/catch boilerplate.
/// <list type="bullet">
///   <item><see cref="ValidationException"/> → 400 Bad Request with field-level errors.</item>
///   <item>Any other <see cref="Exception"/> → 500 Internal Server Error.</item>
/// </list>
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                "Validation failure on {Method} {Path}: {Errors}",
                context.Request.Method,
                context.Request.Path,
                ex.Message);

            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception on {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await HandleGenericExceptionAsync(context);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        ValidationProblemDetails details = new()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7807",
            Instance = context.Request.Path
        };

        foreach (FluentValidation.Results.ValidationFailure failure in ex.Errors)
        {
            if (!details.Errors.ContainsKey(failure.PropertyName))
            {
                details.Errors[failure.PropertyName] = [];
            }

            details.Errors[failure.PropertyName] =
                [.. details.Errors[failure.PropertyName], failure.ErrorMessage];
        }

        await context.Response.WriteAsJsonAsync(details);
    }

    private static async Task HandleGenericExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        ProblemDetails details = context.CreateProblemDetails(
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred.",
            "An unexpected error occurred. Please try again later.",
            "https://tools.ietf.org/html/rfc7807");

        await context.Response.WriteAsJsonAsync(details);
    }
}
