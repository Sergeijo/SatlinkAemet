using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Satlink.Contracts.Dtos.Aemet;

namespace Satlink.Logic;

/// <summary>
/// Read-only query port for fast Dapper-based reads on the AEMET SQLite download store.
/// Belongs to the query side of CQRS; commands still use the keyed
/// <see cref="IRequestsRepository"/>.
/// </summary>
public interface IAemetDownloadQueryRepository
{
    /// <summary>
    /// Returns all AEMET download records ordered by date descending.
    /// <see cref="MarineZonePredictionDto.fechaDescarga"/> is always populated.
    /// </summary>
    Task<List<MarineZonePredictionDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the download record for a given zone and date, or <c>null</c> if not found.
    /// </summary>
    Task<MarineZonePredictionDto?> GetByIdAsync(string zoneId, DateOnly fechaDescarga, CancellationToken cancellationToken);
}
