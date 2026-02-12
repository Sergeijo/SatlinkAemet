using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Implements CRUD operations for <see cref="Request"/> items.
/// </summary>
public sealed class RequestsService : IRequestsService
{
    private readonly IRequestsRepository _requestsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestsService"/> class.
    /// </summary>
    /// <param name="requestsRepository">The requests repository.</param>
    public RequestsService(IRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository;
    }

    /// <inheritdoc />
    public async Task<Result<List<Request>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            List<Request> items = await _requestsRepository.GetAllAsync(cancellationToken);

            // Return items.
            return Result.Ok(items);
        }
        catch (Exception ex)
        {
            // Map unexpected exception.
            return Result.Fail<List<Request>>("Error while retrieving items: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            Request? entity = await _requestsRepository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                // Return failure if not found.
                return Result.Fail<Request>("Request not found.");
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while retrieving item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> CreateAsync(Request request, CancellationToken cancellationToken)
    {
        try
        {
            // Assign id if missing.
            if (string.IsNullOrWhiteSpace(request.id))
            {
                request.id = Guid.NewGuid().ToString("N");
            }

            Request created = await _requestsRepository.CreateAsync(request, cancellationToken);

            // Return created.
            return Result.Ok(created);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while creating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Request>> UpdateAsync(string id, Request request, CancellationToken cancellationToken)
    {
        try
        {
            Request? updated = await _requestsRepository.UpdateAsync(id, request, cancellationToken);

            return updated is null
                ? Result.Fail<Request>("Request not found.")
                : Result.Ok(updated);
        }
        catch (Exception ex)
        {
            return Result.Fail<Request>("Error while updating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            bool deleted = await _requestsRepository.DeleteAsync(id, cancellationToken);

            if (!deleted)
            {
                return Result.Fail("Request not found.");
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("Error while deleting item: " + ex.Message);
        }
    }
}
