namespace DataNative.Storage;

/// <summary>
/// Abstraction for blob/object storage operations.
/// Implemented by provider-specific services (MinIO, Azure Blob, S3, GCS).
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a blob to the specified container.
    /// </summary>
    Task<BlobUploadResult> UploadAsync(
        string containerName,
        string blobName,
        Stream content,
        string? contentType = null,
        IDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob from the specified container.
    /// </summary>
    Task<Stream> DownloadAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob from the specified container.
    /// </summary>
    Task<bool> DeleteAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a blob exists in the specified container.
    /// </summary>
    Task<bool> ExistsAsync(
        string containerName,
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a pre-signed or public URL for a blob.
    /// </summary>
    Task<Uri> GetUrlAsync(
        string containerName,
        string blobName,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists blobs in a container with an optional prefix filter.
    /// </summary>
    IAsyncEnumerable<BlobItem> ListBlobsAsync(
        string containerName,
        string? prefix = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures the specified container exists, creating it if necessary.
    /// </summary>
    Task EnsureContainerExistsAsync(
        string containerName,
        CancellationToken cancellationToken = default);
}
