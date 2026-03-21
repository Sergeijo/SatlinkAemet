using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Satlink.Logic;

namespace Satlink.Api.Middleware;

/// <summary>
/// Runs after <c>UseAuthentication</c> so the JWT is already parsed and
/// <see cref="IUserContext"/> is fully populated via <c>IHttpContextAccessor</c>.
/// <para>
/// Responsibilities:
/// <list type="bullet">
///   <item>Logs every inbound HTTP request with method, path and authenticated user.</item>
///   <item>Logs every outbound response with status code and elapsed time.</item>
///   <item>Exposes <see cref="IUserContext"/> to the MediatR pipeline so that
///         <c>LoggingBehaviour</c> can record <em>who</em> executed each command.</item>
/// </list>
/// </para>
/// <para>
/// <b>IUserContext integration:</b> <see cref="UserContext"/> is a scoped service that
/// reads lazily from <c>IHttpContextAccessor.HttpContext.User</c>. Because
/// <c>UseAuthentication</c> sets <c>HttpContext.User</c> before this middleware
/// runs, any component in the same request scope (controllers, handlers, behaviours)
/// that injects <see cref="IUserContext"/> will automatically see the authenticated
/// identity – no explicit "fill" call is required.
/// </para>
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    // ILogger is safe to inject in the constructor (singleton-like lifetime).
    // IUserContext is scoped and therefore injected via InvokeAsync instead.
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        string method = context.Request.Method;
        string path = context.Request.Path;
        string userId = userContext.UserId ?? "anonymous";
        string email = userContext.Email ?? "anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Path} started | User: {UserId} ({Email})",
            method, path, userId, email);

        Stopwatch sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();

        _logger.LogInformation(
            "HTTP {Method} {Path} finished {StatusCode} in {ElapsedMs}ms | User: {UserId}",
            method, path, context.Response.StatusCode, sw.ElapsedMilliseconds, userId);
    }
}
