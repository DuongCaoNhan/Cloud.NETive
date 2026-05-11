namespace CloudNative.Security.Authorization;

/// <summary>
/// Service for checking user permissions against resources.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Checks if a user has a specific permission on a resource.
    /// </summary>
    Task<bool> HasPermissionAsync(string userId, string permission, string? resourceId = null);

    /// <summary>
    /// Gets all permissions for a user.
    /// </summary>
    Task<IEnumerable<string>> GetPermissionsAsync(string userId);
}
