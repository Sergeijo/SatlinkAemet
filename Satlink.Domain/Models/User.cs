using System;
using System.Collections.Generic;

namespace Satlink.Domain.Models
{
    public class Request
    {
        public Origen origen { get; set; }
        public Situacion situacion { get; set; }
        public Prediccion prediccion { get; set; }
        public string id { get; set; }
        public string nombre { get; set; }
    }

    public class FicheroTemporal
    {
        public string descripcion { get; set; }
        public int estado { get; set; }
        public string datos { get; set; }
        public string metadatos { get; set; }
    }

    public class Origen
    {
        public string productor { get; set; }
        public string web { get; set; }
        public string language { get; set; }
        public string copyright { get; set; }
        public string notaLegal { get; set; }
        public DateTime elaborado { get; set; }
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
    }

    public class Prediccion
    {
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
        public List<Zona> zona { get; set; }
    }

    public class Situacion
    {
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
        public string texto { get; set; }
        public string id { get; set; }
        public string nombre { get; set; }
    }

    public class Zona
    {
        public string texto { get; set; }
        public int id { get; set; }
        public string nombre { get; set; }
    }
}