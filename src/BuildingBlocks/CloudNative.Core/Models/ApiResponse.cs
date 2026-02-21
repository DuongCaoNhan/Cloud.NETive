namespace CloudNative.Core.Models;

/// <summary>Standard envelope for all API responses.</summary>
public record ApiResponse<T>(bool Success, T? Data, string? Message = null, IEnumerable<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null)
        => new(true, data, message);

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
        => new(false, default, message, errors);
}
