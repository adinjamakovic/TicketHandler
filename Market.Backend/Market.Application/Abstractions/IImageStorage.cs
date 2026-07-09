using Microsoft.AspNetCore.Http;

namespace Market.Application.Abstractions;

/// <summary>
/// Service for storing entity images in Azure Blob Storage and exposing public URLs.
/// </summary>
public interface IImageStorage
{
    /// <summary>
    /// Saves an uploaded image when provided; returns null when no file is supplied.
    /// </summary>
    Task<string?> SaveAsync(ImageStorageCategory category, IFormFile? image, CancellationToken ct = default);

    /// <summary>
    /// Deletes the blob for a stored path, if it exists.
    /// </summary>
    Task DeleteIfExistsAsync(ImageStorageCategory category, string? storedPath, CancellationToken ct = default);

    /// <summary>
    /// Replaces the stored image only when a new file is uploaded; otherwise returns the current path unchanged.
    /// </summary>
    Task<string?> ReplaceIfUploadedAsync(
        ImageStorageCategory category,
        string? currentStoredPath,
        IFormFile? newImage,
        CancellationToken ct = default);

    /// <summary>
    /// Normalizes a stored blob name to its public blob URL.
    /// </summary>
    string? ToPublicPath(ImageStorageCategory category, string? storedPath);
}
