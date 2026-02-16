namespace Satlink.Api.Dtos.Requests;

/// <summary>
/// Represents the payload to update a request.
/// </summary>
public sealed class UpdateRequestDto
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
}
