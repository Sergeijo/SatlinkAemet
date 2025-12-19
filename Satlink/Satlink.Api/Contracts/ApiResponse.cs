namespace Satlink.Api.Contracts;

/// <summary>
/// Represents a standard API response wrapper.
/// </summary>
/// <typeparam name="T">The response payload type.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
    /// </summary>
    /// <param name="data">The payload.</param>
    /// <param name="message">A human-readable message.</param>
    public ApiResponse(T? data, string? message)
    {
        Data = data;
        Message = message;
    }

    /// <summary>
    /// Gets the payload.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets an optional message.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="data">The payload.</param>
    /// <param name="message">An optional message.</param>
    /// <returns>The created response wrapper.</returns>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        // Wrap successful payload and message.
        return new ApiResponse<T>(data, message);
    }
}
