using System;
using System.Collections.Generic;

namespace Satlink.Contracts.Aemet;

public sealed class AemetMarineZonePredictionDto
{
    public AemetOrigenDto? origen { get; set; }

    public AemetSituacionDto? situacion { get; set; }

    public AemetPrediccionDto? prediccion { get; set; }

    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;
}

public sealed class AemetFicheroTemporalDto
{
    public string descripcion { get; set; } = string.Empty;

    public int estado { get; set; }

    public string datos { get; set; } = string.Empty;

    public string metadatos { get; set; } = string.Empty;
}

public sealed class AemetOrigenDto
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

public sealed class AemetPrediccionDto
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public List<AemetZonaDto>? zona { get; set; }
}

public sealed class AemetSituacionDto
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public string texto { get; set; } = string.Empty;

    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;
}

public sealed class AemetZonaDto
{
    public string texto { get; set; } = string.Empty;

    public int id { get; set; }

    public string nombre { get; set; } = string.Empty;
}
