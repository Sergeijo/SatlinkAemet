using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Satlink.Logic.CQRS.Behaviours;

/// <summary>
/// Logs the start and end of every MediatR request with elapsed time and the
/// identity of the user who triggered it (via <see cref="IUserContext"/>).
/// </summary>
public sealed class LoggingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly IUserContext _userContext;

    public LoggingBehaviour(
        ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        string userId = _userContext.UserId ?? "anonymous";
        string email = _userContext.Email ?? "anonymous";

        _logger.LogInformation(
            "Executing CQRS {RequestName} | User: {UserId} ({Email})",
            requestName, userId, email);

        Stopwatch sw = Stopwatch.StartNew();

        try
        {
            TResponse response = await next(cancellationToken);
            sw.Stop();

            _logger.LogInformation(
                "Completed CQRS {RequestName} in {ElapsedMs}ms | User: {UserId}",
                requestName, sw.ElapsedMilliseconds, userId);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            _logger.LogWarning(
                ex,
                "Failed CQRS {RequestName} in {ElapsedMs}ms | User: {UserId}",
                requestName, sw.ElapsedMilliseconds, userId);

            throw;
        }
    }
}
