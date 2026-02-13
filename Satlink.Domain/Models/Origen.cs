using System;

namespace Satlink.Domain.Models;

public class Origen
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
