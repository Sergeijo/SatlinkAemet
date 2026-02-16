using System;
using System.Collections.Generic;

namespace Satlink.Contracts.Dtos.Requests;

public sealed class RequestDto
{
    public RequestOrigenDto Origen { get; init; } = new RequestOrigenDto();

    public RequestSituacionDto Situacion { get; init; } = new RequestSituacionDto();

    public RequestPrediccionDto Prediccion { get; init; } = new RequestPrediccionDto();

    public string Id { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;
}

public sealed class RequestOrigenDto
{
    public string Productor { get; init; } = string.Empty;

    public string Web { get; init; } = string.Empty;

    public string Language { get; init; } = string.Empty;

    public string Copyright { get; init; } = string.Empty;

    public string NotaLegal { get; init; } = string.Empty;

    public DateTime Elaborado { get; init; }

    public DateTime Inicio { get; init; }

    public DateTime Fin { get; init; }
}

public sealed class RequestSituacionDto
{
    public DateTime Inicio { get; init; }

    public DateTime Fin { get; init; }

    public string Texto { get; init; } = string.Empty;

    public string Id { get; init; } = string.Empty;

    public string Nombre { get; init; } = string.Empty;
}

public sealed class RequestPrediccionDto
{
    public DateTime Inicio { get; init; }

    public DateTime Fin { get; init; }

    public IReadOnlyList<RequestZonaDto> Zona { get; init; } = Array.Empty<RequestZonaDto>();
}

public sealed class RequestZonaDto
{
    public string Texto { get; init; } = string.Empty;

    public int Id { get; init; }

    public string Nombre { get; init; } = string.Empty;
}
