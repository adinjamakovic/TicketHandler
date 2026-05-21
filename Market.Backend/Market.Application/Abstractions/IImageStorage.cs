using Microsoft.AspNetCore.Http;

namespace Market.Application.Abstractions;

/// <summary>
/// Service for storing entity images under wwwroot and exposing web-relative paths.
/// </summary>
public interface IImageStorage
{
    /// <summary>
    /// Saves an uploaded image when provided; returns null when no file is supplied.
    /// </summary>
    Task<string?> SaveAsync(ImageStorageCategory category, IFormFile? image, CancellationToken ct = default);

    /// <summary>
    /// Deletes the physical file for a stored path, if it exists.
    /// </summary>
    void DeleteIfExists(ImageStorageCategory category, string? storedPath);

    /// <summary>
    /// Replaces the stored image only when a new file is uploaded; otherwise returns the current path unchanged.
    /// </summary>
    Task<string?> ReplaceIfUploadedAsync(
        ImageStorageCategory category,
        string? currentStoredPath,
        IFormFile? newImage,
        CancellationToken ct = default);

    /// <summary>
    /// Normalizes a stored value to a public URL path such as <c>/Upload/Events/{file}</c>.
    /// </summary>
    string? ToPublicPath(ImageStorageCategory category, string? storedPath);
}
