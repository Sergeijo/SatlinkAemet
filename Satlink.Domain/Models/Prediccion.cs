using System;
using System.Collections.Generic;

namespace Satlink.Domain.Models;

public class Prediccion
{
    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public List<Zona> zona { get; set; } = new List<Zona>();
}
