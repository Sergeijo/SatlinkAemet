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

    /// <summary>
    /// Serializes a .NET instance to a JSON string.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>JSON representation.</returns>
    string Serialize<T>(T value);
}
