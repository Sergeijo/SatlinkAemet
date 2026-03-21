using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Satlink.Logic.CQRS.Behaviours;

/// <summary>
/// Outermost pipeline behaviour.
/// Catches any unhandled exception that escapes the inner pipeline,
/// logs it with full context and re-throws so that the global exception
/// middleware can convert it into an appropriate HTTP response.
/// <para>
/// <b>Pipeline order (outermost → handler):</b>
/// ExceptionBehaviour → LoggingBehaviour → ValidationBehaviour → TransactionBehaviour → Handler
/// </para>
/// </summary>
public sealed class ExceptionBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionBehaviour<TRequest, TResponse>> _logger;

    public ExceptionBehaviour(ILogger<ExceptionBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (FluentValidation.ValidationException)
        {
            // Validation failures are expected; let them bubble up without logging as errors.
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception while processing {RequestName}. Request: {@Request}",
                typeof(TRequest).Name,
                request);

            throw;
        }
    }
}
