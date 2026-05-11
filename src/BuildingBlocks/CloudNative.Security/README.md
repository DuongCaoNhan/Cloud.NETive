# CloudNative.Security

Security building block providing JWT authentication, role-based authorization, and AES-256 encryption abstractions for CloudNative services.

## Overview

| Namespace | Purpose |
|-----------|---------|
| `CloudNative.Security.Authentication` | JWT token generation and validation |
| `CloudNative.Security.Authorization` | Permission checks, roles, and policies |
| `CloudNative.Security.Encryption` | AES-256 encryption and SHA-256 hashing |

## Authentication

### ITokenService

Implement `ITokenService` to provide JWT access and refresh token management.

```csharp
public interface ITokenService
{
    string GenerateAccessToken(TokenRequest request);
    string GenerateRefreshToken();
    TokenValidationResult ValidateToken(string token);
}
```

**TokenRequest**

| Property | Type | Description |
|----------|------|-------------|
| `UserId` | `string` | Unique user identifier |
| `Email` | `string` | User email address |
| `Roles` | `IEnumerable<string>` | Assigned roles |
| `Claims` | `IDictionary<string, string>` | Additional custom claims |

### JwtOptions

Configure via `appsettings.json` under the `"Jwt"` section:

```json
{
  "Jwt": {
    "SecretKey": "<store-in-key-vault>",
    "Issuer": "CloudNative",
    "Audience": "CloudNative",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

> **Security:** Never store `SecretKey` in source control. Use Azure Key Vault or environment secrets.

Register options in `Program.cs`:

```csharp
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
```

## Authorization

### IPermissionService

Resource-level permission checks:

```csharp
bool canEdit = await permissionService.HasPermissionAsync(userId, "edit", resourceId);
IEnumerable<string> perms = await permissionService.GetPermissionsAsync(userId);
```

### Roles

```csharp
// Predefined role constants
Roles.Admin     // "Admin"
Roles.Manager   // "Manager"
Roles.User      // "User"
Roles.ReadOnly  // "ReadOnly"
```

### Policies

```csharp
// Predefined policy constants
Policies.RequireAdmin    // "RequireAdmin"
Policies.RequireManager  // "RequireManager"
Policies.RequireUser     // "RequireUser"
Policies.CanRead         // "CanRead"
Policies.CanWrite        // "CanWrite"
Policies.CanDelete       // "CanDelete"
```

Apply policies on controllers or endpoints:

```csharp
[Authorize(Policy = Policies.CanWrite)]
public async Task<IActionResult> Create(...)
```

## Encryption

### IEncryptionService

AES-256 encryption and SHA-256 hashing:

```csharp
string cipher = encryptionService.Encrypt(plainText, key);
string plain  = encryptionService.Decrypt(cipher, key);
string hash   = encryptionService.HashSha256(input);
```

### EncryptionOptions

```json
{
  "Encryption": {
    "MasterKey": "<store-in-key-vault>",
    "KeyRotationDays": 90
  }
}
```

Register options in `Program.cs`:

```csharp
builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection(EncryptionOptions.SectionName));
```

## Package References

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.0 | JWT bearer middleware |
| `System.IdentityModel.Tokens.Jwt` | 8.9.0 | JWT token creation and validation |
