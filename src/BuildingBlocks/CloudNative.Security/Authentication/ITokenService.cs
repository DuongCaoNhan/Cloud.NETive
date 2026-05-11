namespace CloudNative.Security.Authentication;

/// <summary>
/// JWT token generation and validation service.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    string GenerateAccessToken(TokenRequest request);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a token and returns the claims principal.
    /// </summary>
    TokenValidationResult ValidateToken(string token);
}

/// <summary>
/// Request model for token generation.
/// </summary>
public class TokenRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
    public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Result of token validation.
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string> Roles { get; set; } = [];
    public string? Error { get; set; }
}
