using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Satlink.Contracts.Dtos.Requests;
using Satlink.Logic;

namespace Satlink.Infrastructure.Dapper;

/// <summary>
/// Dapper-based implementation of <see cref="IRequestsQueryRepository"/>.
/// Reads go directly from SQL to DTOs, bypassing EF Core change tracking
/// and entity materialization for maximum read performance.
/// </summary>
internal sealed class RequestsDapperQueryRepository : IRequestsQueryRepository
{
    // Column aliases map 1:1 to RequestFlatRow so Dapper resolves them automatically.
    private const string SelectColumns = """
        r.id                 AS Id,
        r.nombre             AS Nombre,
        r.origen_productor   AS Origen_Productor,
        r.origen_web         AS Origen_Web,
        r.origen_language    AS Origen_Language,
        r.origen_copyright   AS Origen_Copyright,
        r.origen_notaLegal   AS Origen_NotaLegal,
        r.origen_elaborado   AS Origen_Elaborado,
        r.origen_inicio      AS Origen_Inicio,
        r.origen_fin         AS Origen_Fin,
        r.situacion_inicio   AS Situacion_Inicio,
        r.situacion_fin      AS Situacion_Fin,
        r.situacion_texto    AS Situacion_Texto,
        r.situacion_id       AS Situacion_Id,
        r.situacion_nombre   AS Situacion_Nombre,
        r.prediccion_inicio  AS Prediccion_Inicio,
        r.prediccion_fin     AS Prediccion_Fin
        """;

    private readonly ISqlConnectionFactory _connectionFactory;

    public RequestsDapperQueryRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<List<RequestDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        string sql = $"""
            SELECT {SelectColumns}
            FROM   zonePredictionsItems AS r
            ORDER  BY r.id
            """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        IEnumerable<RequestFlatRow> rows = await connection.QueryAsync<RequestFlatRow>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return rows.Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<RequestDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        string sql = $"""
            SELECT {SelectColumns}
            FROM   zonePredictionsItems AS r
            WHERE  r.id = @Id
            """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        RequestFlatRow? row = await connection.QuerySingleOrDefaultAsync<RequestFlatRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return row is null ? null : MapToDto(row);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static RequestDto MapToDto(RequestFlatRow row) =>
        new RequestDto
        {
            Id = row.Id,
            Nombre = row.Nombre,
            Origen = new RequestOrigenDto
            {
                Productor = row.Origen_Productor ?? string.Empty,
                Web = row.Origen_Web ?? string.Empty,
                Language = row.Origen_Language ?? string.Empty,
                Copyright = row.Origen_Copyright ?? string.Empty,
                NotaLegal = row.Origen_NotaLegal ?? string.Empty,
                Elaborado = row.Origen_Elaborado.GetValueOrDefault(),
                Inicio = row.Origen_Inicio.GetValueOrDefault(),
                Fin = row.Origen_Fin.GetValueOrDefault()
            },
            Situacion = new RequestSituacionDto
            {
                Inicio = row.Situacion_Inicio.GetValueOrDefault(),
                Fin = row.Situacion_Fin.GetValueOrDefault(),
                Texto = row.Situacion_Texto ?? string.Empty,
                Id = row.Situacion_Id ?? string.Empty,
                Nombre = row.Situacion_Nombre ?? string.Empty
            },
            Prediccion = new RequestPrediccionDto
            {
                Inicio = row.Prediccion_Inicio.GetValueOrDefault(),
                Fin = row.Prediccion_Fin.GetValueOrDefault(),
                Zona = System.Array.Empty<RequestZonaDto>()
            }
        };

    /// <summary>
    /// Flat row model that Dapper maps directly from the SQL result set.
    /// All columns are nullable to handle rows created before full schema population.
    /// </summary>
    private sealed class RequestFlatRow
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        // Origen columns
        public string? Origen_Productor { get; set; }
        public string? Origen_Web { get; set; }
        public string? Origen_Language { get; set; }
        public string? Origen_Copyright { get; set; }
        public string? Origen_NotaLegal { get; set; }
        public System.DateTime? Origen_Elaborado { get; set; }
        public System.DateTime? Origen_Inicio { get; set; }
        public System.DateTime? Origen_Fin { get; set; }

        // Situacion columns
        public System.DateTime? Situacion_Inicio { get; set; }
        public System.DateTime? Situacion_Fin { get; set; }
        public string? Situacion_Texto { get; set; }
        public string? Situacion_Id { get; set; }
        public string? Situacion_Nombre { get; set; }

        // Prediccion scalar columns (zona collection lives in a separate table)
        public System.DateTime? Prediccion_Inicio { get; set; }
        public System.DateTime? Prediccion_Fin { get; set; }
    }
}
