namespace CloudNative.Security.Authorization;

/// <summary>
/// Defines standard application roles.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";
    public const string ReadOnly = "ReadOnly";
}

/// <summary>
/// Defines standard application policies.
/// </summary>
public static class Policies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireManager = "RequireManager";
    public const string RequireUser = "RequireUser";
    public const string CanRead = "CanRead";
    public const string CanWrite = "CanWrite";
    public const string CanDelete = "CanDelete";
}
