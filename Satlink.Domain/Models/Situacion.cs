using System;

namespace Satlink.Domain.Models;

public class Situacion
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public string texto { get; set; } = string.Empty;

    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;
}
