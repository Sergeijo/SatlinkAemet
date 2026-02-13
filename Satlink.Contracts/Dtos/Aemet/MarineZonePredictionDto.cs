using System;
using System.Collections.Generic;

namespace Satlink.Contracts.Dtos.Aemet;

public sealed class MarineZonePredictionDto
{
    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;

    public OrigenDto origen { get; set; } = new OrigenDto();

    public SituacionDto situacion { get; set; } = new SituacionDto();

    public PrediccionDto prediccion { get; set; } = new PrediccionDto();
}

public sealed class OrigenDto
{
    public string productor { get; set; } = string.Empty;

    public string web { get; set; } = string.Empty;

    public string language { get; set; } = string.Empty;

    public string copyright { get; set; } = string.Empty;

    public string notaLegal { get; set; } = string.Empty;

    public DateTime elaborado { get; set; }

    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }
}

public sealed class PrediccionDto
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public List<ZonaDto> zona { get; set; } = new List<ZonaDto>();
}

public sealed class SituacionDto
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public string texto { get; set; } = string.Empty;

    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;
}

public sealed class ZonaDto
{
    public int id { get; set; }

    public string nombre { get; set; } = string.Empty;

    public string texto { get; set; } = string.Empty;
}
