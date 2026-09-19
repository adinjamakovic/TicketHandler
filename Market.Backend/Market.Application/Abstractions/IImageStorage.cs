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
    /// Deletes the blob for a stored path, if it exists. Callers removing an entity run this only
    /// after the delete is saved, so a failure can never strip the image off a surviving row.
    /// </summary>
    Task DeleteIfExistsAsync(ImageStorageCategory category, string? storedPath, CancellationToken ct = default);

    /// <summary>
    /// Uploads a replacement image and returns its stored path; returns the current path unchanged
    /// when no new file is supplied. The old blob is deliberately left in place — callers drop it
    /// with <see cref="DeleteIfExistsAsync"/> only after the new path is saved, so neither a failed
    /// upload nor a failed save can leave a row pointing at a blob that is already gone.
    /// </summary>
    Task<string?> SaveIfUploadedAsync(
        ImageStorageCategory category,
        string? currentStoredPath,
        IFormFile? newImage,
        CancellationToken ct = default);

    /// <summary>
    /// Normalizes a stored blob name to its public blob URL.
    /// </summary>
    string? ToPublicPath(ImageStorageCategory category, string? storedPath);
}
