using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using Newtonsoft.Json;

using Satlink.Contracts.Dtos.Aemet;
using Satlink.Logic;

namespace Satlink.Infrastructure.Dapper;

/// <summary>
/// Dapper-based implementation of <see cref="IAemetDownloadQueryRepository"/>.
/// Reads go directly from SQLite to DTOs, bypassing EF Core for maximum throughput.
/// </summary>
internal sealed class AemetDownloadDapperQueryRepository : IAemetDownloadQueryRepository
{
    // Column aliases map 1:1 to AemetDownloadFlatRow so Dapper resolves them automatically.
    private const string SelectColumns = """
        r.id                    AS Id,
        r.nombre                AS Nombre,
        r.fecha_descarga        AS FechaDescarga,
        r.origen_productor      AS Origen_Productor,
        r.origen_web            AS Origen_Web,
        r.origen_language       AS Origen_Language,
        r.origen_copyright      AS Origen_Copyright,
        r.origen_notaLegal      AS Origen_NotaLegal,
        r.origen_elaborado      AS Origen_Elaborado,
        r.origen_inicio         AS Origen_Inicio,
        r.origen_fin            AS Origen_Fin,
        r.situacion_inicio      AS Situacion_Inicio,
        r.situacion_fin         AS Situacion_Fin,
        r.situacion_texto       AS Situacion_Texto,
        r.situacion_id          AS Situacion_Id,
        r.situacion_nombre      AS Situacion_Nombre,
        r.prediccion_inicio     AS Prediccion_Inicio,
        r.prediccion_fin        AS Prediccion_Fin,
        r.prediccion_zona       AS Prediccion_Zona
        """;

    private readonly ISqliteConnectionFactory _connectionFactory;

    public AemetDownloadDapperQueryRepository(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public async Task<List<MarineZonePredictionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        string sql = $"""
            SELECT {SelectColumns}
            FROM   AemetDownloads AS r
            ORDER  BY r.fecha_descarga DESC, r.id
            """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        IEnumerable<AemetDownloadFlatRow> rows = await connection.QueryAsync<AemetDownloadFlatRow>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return rows.Select(MapToDto).ToList();
    }

    /// <inheritdoc />
    public async Task<MarineZonePredictionDto?> GetByIdAsync(
        string zoneId,
        DateOnly fechaDescarga,
        CancellationToken cancellationToken)
    {
        string sql = $"""
            SELECT {SelectColumns}
            FROM   AemetDownloads AS r
            WHERE  r.id             = @ZoneId
            AND    r.fecha_descarga = @FechaDescarga
            """;

        using IDbConnection connection = _connectionFactory.CreateConnection();

        AemetDownloadFlatRow? row = await connection.QuerySingleOrDefaultAsync<AemetDownloadFlatRow>(
            new CommandDefinition(
                sql,
                new { ZoneId = zoneId, FechaDescarga = fechaDescarga.ToString("yyyy-MM-dd") },
                cancellationToken: cancellationToken));

        return row is null ? null : MapToDto(row);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static MarineZonePredictionDto MapToDto(AemetDownloadFlatRow row) =>
        new MarineZonePredictionDto
        {
            id = row.Id,
            nombre = row.Nombre,
            fechaDescarga = DateOnly.Parse(row.FechaDescarga),
            origen = new OrigenDto
            {
                productor = row.Origen_Productor ?? string.Empty,
                web = row.Origen_Web ?? string.Empty,
                language = row.Origen_Language ?? string.Empty,
                copyright = row.Origen_Copyright ?? string.Empty,
                notaLegal = row.Origen_NotaLegal ?? string.Empty,
                elaborado = row.Origen_Elaborado.GetValueOrDefault(),
                inicio = row.Origen_Inicio.GetValueOrDefault(),
                fin = row.Origen_Fin.GetValueOrDefault()
            },
            situacion = new SituacionDto
            {
                inicio = row.Situacion_Inicio.GetValueOrDefault(),
                fin = row.Situacion_Fin.GetValueOrDefault(),
                texto = row.Situacion_Texto ?? string.Empty,
                id = row.Situacion_Id ?? string.Empty,
                nombre = row.Situacion_Nombre ?? string.Empty
            },
            prediccion = new PrediccionDto
            {
                inicio = row.Prediccion_Inicio.GetValueOrDefault(),
                fin = row.Prediccion_Fin.GetValueOrDefault(),
                zona = DeserializeZonas(row.Prediccion_Zona)
            }
        };

    private static List<ZonaDto> DeserializeZonas(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<ZonaDto>();
        }

        List<ZonaRow>? rows = JsonConvert.DeserializeObject<List<ZonaRow>>(json);

        if (rows is null || rows.Count == 0)
        {
            return new List<ZonaDto>();
        }

        List<ZonaDto> mapped = new List<ZonaDto>(rows.Count);

        foreach (ZonaRow z in rows)
        {
            mapped.Add(new ZonaDto { id = z.id, nombre = z.nombre, texto = z.texto });
        }

        return mapped;
    }

    /// <summary>
    /// Flat row that Dapper maps directly from the SQLite result set.
    /// </summary>
    private sealed class AemetDownloadFlatRow
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string FechaDescarga { get; set; } = string.Empty; // ISO-8601 text from SQLite

        public string? Origen_Productor { get; set; }
        public string? Origen_Web { get; set; }
        public string? Origen_Language { get; set; }
        public string? Origen_Copyright { get; set; }
        public string? Origen_NotaLegal { get; set; }
        public DateTime? Origen_Elaborado { get; set; }
        public DateTime? Origen_Inicio { get; set; }
        public DateTime? Origen_Fin { get; set; }

        public DateTime? Situacion_Inicio { get; set; }
        public DateTime? Situacion_Fin { get; set; }
        public string? Situacion_Texto { get; set; }
        public string? Situacion_Id { get; set; }
        public string? Situacion_Nombre { get; set; }

        public DateTime? Prediccion_Inicio { get; set; }
        public DateTime? Prediccion_Fin { get; set; }
        public string? Prediccion_Zona { get; set; } // JSON blob
    }

    /// <summary>
    /// Matches the lowercase property names used when <see cref="Satlink.Domain.Models.Zona"/>
    /// was serialized to JSON by EF Core's value converter.
    /// </summary>
    private sealed class ZonaRow
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string texto { get; set; } = string.Empty;
    }
}