namespace DataNative.Storage;

/// <summary>
/// Represents a blob item in storage listing results.
/// </summary>
public sealed record BlobItem(
    string Name,
    long? Size,
    string? ContentType,
    DateTimeOffset? LastModified,
    IDictionary<string, string>? Metadata = null);

/// <summary>
/// Result of a blob upload operation.
/// </summary>
public sealed record BlobUploadResult(
    string ContainerName,
    string BlobName,
    Uri? Uri,
    string? ETag,
    long Size);

/// <summary>
/// Configuration options for blob storage.
/// </summary>
public sealed class BlobStorageOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Storage";

    /// <summary>
    /// Infrastructure provider: OnPremise, Azure, AWS, GCP.
    /// </summary>
    public string Provider { get; set; } = "OnPremise";

    /// <summary>
    /// Connection string for the storage provider.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Prefix applied to all container names.
    /// </summary>
    public string ContainerPrefix { get; set; } = "datanative";

    /// <summary>
    /// Default expiry duration for pre-signed URLs.
    /// </summary>
    public TimeSpan DefaultUrlExpiry { get; set; } = TimeSpan.FromHours(1);
}
