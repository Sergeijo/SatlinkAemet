using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;
using Satlink.Logic.CQRS.Behaviours;
using Satlink.Logic.Messaging.Events;

namespace Satlink.Logic.CQRS.Requests.Commands;

/// <summary>
/// Command to create a new request.
/// Implements <see cref="ITransactionalCommand"/> so the handler runs inside a
/// SQL Server transaction (managed by <see cref="TransactionBehaviour{TRequest,TResponse}"/>).
/// On success, a <see cref="RequestCreatedIntegrationEvent"/> is published via the
/// EF Core outbox – atomically with the business row.
/// </summary>
public sealed record CreateRequestCommand(string Nombre)
    : IRequest<Result<RequestDto>>, ITransactionalCommand;

/// <summary>
/// Handler for <see cref="CreateRequestCommand"/>.
/// </summary>
public sealed class CreateRequestCommandHandler
    : IRequestHandler<CreateRequestCommand, Result<RequestDto>>
{
    private readonly IRequestsService _requestsService;
    private readonly IEventBus _eventBus;

    public CreateRequestCommandHandler(IRequestsService requestsService, IEventBus eventBus)
    {
        _requestsService = requestsService;
        _eventBus = eventBus;
    }

    public async Task<Result<RequestDto>> Handle(
        CreateRequestCommand request,
        CancellationToken cancellationToken)
    {
        Result<RequestDto> result = await _requestsService
            .CreateAsync(request.Nombre, cancellationToken);

        if (result.Success)
        {
            await _eventBus.PublishAsync(
                new RequestCreatedIntegrationEvent
                {
                    RequestId = result.Value.Id,
                    Nombre = result.Value.Nombre,
                    CreatedAt = DateTime.UtcNow
                },
                cancellationToken);
        }

        return result;
    }
}

