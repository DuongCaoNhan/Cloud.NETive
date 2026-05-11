using System.Security.Cryptography;
using System.Text;

namespace DataNative.Utils.Helpers;

/// <summary>
/// File system helper utilities
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Ensure directory exists, create if not
    /// </summary>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Get safe filename by removing invalid characters
    /// </summary>
    public static string GetSafeFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return "unnamed_file";

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFileName = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
        
        return string.IsNullOrEmpty(safeFileName) ? "unnamed_file" : safeFileName;
    }

    /// <summary>
    /// Get temporary file path with specific extension
    /// </summary>
    public static string GetTempFilePath(string extension = ".tmp")
    {
        var tempPath = Path.GetTempPath();
        var fileName = $"{Guid.NewGuid()}{extension}";
        return Path.Combine(tempPath, fileName);
    }

    /// <summary>
    /// Read file with retry mechanism
    /// </summary>
    public static async Task<string> ReadFileWithRetryAsync(string filePath, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (IOException) when (i < maxRetries - 1)
            {
                await Task.Delay(100 * (i + 1)); // Progressive delay
            }
        }
        
        throw new IOException($"Failed to read file after {maxRetries} attempts: {filePath}");
    }
}

/// <summary>
/// Cryptography helper utilities
/// </summary>
public static class CryptographyHelper
{
    /// <summary>
    /// Generate MD5 hash of string
    /// </summary>
    public static string GenerateMD5Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Generate SHA256 hash of string
    /// </summary>
    public static string GenerateSHA256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Generate random string of specified length
    /// </summary>
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

/// <summary>
/// Validation helper utilities
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validate URL format
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Validate email format
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if string contains only alphanumeric characters
    /// </summary>
    public static bool IsAlphanumeric(string input)
    {
        return !string.IsNullOrEmpty(input) && input.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Validate JSON format
    /// </summary>
    public static bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return false;

        try
        {
            System.Text.Json.JsonDocument.Parse(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
