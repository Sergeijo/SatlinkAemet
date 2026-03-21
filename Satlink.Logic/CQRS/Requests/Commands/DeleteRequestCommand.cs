using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Logic.CQRS.Behaviours;

namespace Satlink.Logic.CQRS.Requests.Commands;

/// <summary>
/// Command to delete a request.
/// Implements <see cref="ITransactionalCommand"/> so the operation is atomic.
/// </summary>
public sealed record DeleteRequestCommand(string Id)
    : IRequest<Result>, ITransactionalCommand;

/// <summary>
/// Handler for <see cref="DeleteRequestCommand"/>.
/// </summary>
public sealed class DeleteRequestCommandHandler
    : IRequestHandler<DeleteRequestCommand, Result>
{
    private readonly IRequestsService _requestsService;

    public DeleteRequestCommandHandler(IRequestsService requestsService)
    {
        _requestsService = requestsService;
    }

    public Task<Result> Handle(
        DeleteRequestCommand request,
        CancellationToken cancellationToken)
        => _requestsService.DeleteAsync(request.Id, cancellationToken);
}
