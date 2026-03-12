using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Satlink.Logic.CQRS.AemetValues.Commands;

/// <summary>
/// Command to delete an AEMET download identified by <see cref="ZoneId"/> +
/// <see cref="FechaDescarga"/>.
/// </summary>
public sealed record DeleteAemetDownloadCommand(string ZoneId, DateOnly FechaDescarga)
    : IRequest<Result>;

/// <summary>
/// Handler for <see cref="DeleteAemetDownloadCommand"/>.
/// </summary>
public sealed class DeleteAemetDownloadCommandHandler
    : IRequestHandler<DeleteAemetDownloadCommand, Result>
{
    private readonly IRequestsRepository _sqliteRepository;

    public DeleteAemetDownloadCommandHandler(
        [FromKeyedServices("Sqlite")] IRequestsRepository sqliteRepository)
    {
        _sqliteRepository = sqliteRepository;
    }

    public async Task<Result> Handle(
        DeleteAemetDownloadCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _sqliteRepository
                .DeleteAsync(request.ZoneId, request.FechaDescarga, cancellationToken)
                .ConfigureAwait(false);

            return deleted
                ? Result.Ok()
                : Result.Fail("AEMET download not found.");
        }
        catch (Exception ex)
        {
            return Result.Fail("Error deleting AEMET download: " + ex.Message);
        }
    }
}
