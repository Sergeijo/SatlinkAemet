using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Satlink.Contracts.Dtos.Requests;

namespace Satlink.Logic.CQRS.Requests.Commands;

/// <summary>
/// Command to create a new request.
/// </summary>
public sealed record CreateRequestCommand(string Nombre) : IRequest<Result<RequestDto>>;

/// <summary>
/// Handler for <see cref="CreateRequestCommand"/>.
/// </summary>
public sealed class CreateRequestCommandHandler
    : IRequestHandler<CreateRequestCommand, Result<RequestDto>>
{
    private readonly IRequestsService _requestsService;

    public CreateRequestCommandHandler(IRequestsService requestsService)
    {
        _requestsService = requestsService;
    }

    public Task<Result<RequestDto>> Handle(
        CreateRequestCommand request,
        CancellationToken cancellationToken)
        => _requestsService.CreateAsync(request.Nombre, cancellationToken);
}
