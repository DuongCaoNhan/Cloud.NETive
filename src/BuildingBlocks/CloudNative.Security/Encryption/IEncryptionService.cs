using System.Security.Cryptography;
using System.Text;

namespace CloudNative.Security.Encryption;

/// <summary>
/// Provides data encryption and hashing utilities.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts plaintext using AES-256.
    /// </summary>
    string Encrypt(string plainText, string key);

    /// <summary>
    /// Decrypts ciphertext using AES-256.
    /// </summary>
    string Decrypt(string cipherText, string key);

    /// <summary>
    /// Computes a SHA-256 hash of the input.
    /// </summary>
    string HashSha256(string input);
}

/// <summary>
/// Configuration for encryption operations.
/// </summary>
public class EncryptionOptions
{
    public const string SectionName = "Encryption";

    /// <summary>
    /// Master encryption key (should be stored in Key Vault).
    /// </summary>
    public string MasterKey { get; set; } = string.Empty;

    /// <summary>
    /// Key rotation interval in days.
    /// </summary>
    public int KeyRotationDays { get; set; } = 90;
}
