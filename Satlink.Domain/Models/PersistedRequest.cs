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
}
