using System;

namespace Satlink.Domain.Models;

/// <summary>
/// Persistence model used by EF Core for CRUD endpoints.
/// </summary>
public class PersistedRequest
{
    public Origen origen { get; set; } = new Origen();

    public Situacion situacion { get; set; } = new Situacion();

    public Prediccion prediccion { get; set; } = new Prediccion();

    public string id { get; set; } = string.Empty;

    public string nombre { get; set; } = string.Empty;

    /// <summary>
    /// Download date (without time). Used as part of the composite unique key
    /// in the SQLite AEMET downloads store.
    /// </summary>
    public DateOnly? FechaDescarga { get; set; }
}
