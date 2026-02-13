using System;

namespace Satlink.Logic;

/// <summary>
/// Abstraction for JSON serialization used by application services.
/// </summary>
public interface IAemetJsonSerializer
{
    /// <summary>
    /// Deserializes a JSON payload into a .NET type.
    /// </summary>
    /// <typeparam name="T">Target type.</typeparam>
    /// <param name="json">JSON payload.</param>
    /// <returns>Deserialized instance or null if it cannot be deserialized.</returns>
    T? Deserialize<T>(string json);
}
