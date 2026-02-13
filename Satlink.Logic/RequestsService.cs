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
    public async Task<Result<List<PersistedRequest>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            List<PersistedRequest> items = await _requestsRepository.GetAllAsync(cancellationToken);

            // Return items.
            return Result.Ok(items);
        }
        catch (Exception ex)
        {
            // Map unexpected exception.
            return Result.Fail<List<PersistedRequest>>("Error while retrieving items: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<PersistedRequest>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest? entity = await _requestsRepository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                // Return failure if not found.
                return Result.Fail<PersistedRequest>("Request not found.");
            }

            return Result.Ok(entity);
        }
        catch (Exception ex)
        {
            return Result.Fail<PersistedRequest>("Error while retrieving item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<PersistedRequest>> CreateAsync(PersistedRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Assign id if missing.
            if (string.IsNullOrWhiteSpace(request.id))
            {
                request.id = Guid.NewGuid().ToString("N");
            }

            PersistedRequest created = await _requestsRepository.CreateAsync(request, cancellationToken);

            // Return created.
            return Result.Ok(created);
        }
        catch (Exception ex)
        {
            return Result.Fail<PersistedRequest>("Error while creating item: " + ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<Result<PersistedRequest>> UpdateAsync(string id, PersistedRequest request, CancellationToken cancellationToken)
    {
        try
        {
            PersistedRequest? updated = await _requestsRepository.UpdateAsync(id, request, cancellationToken);

            return updated is null
                ? Result.Fail<PersistedRequest>("Request not found.")
                : Result.Ok(updated);
        }
        catch (Exception ex)
        {
            return Result.Fail<PersistedRequest>("Error while updating item: " + ex.Message);
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
