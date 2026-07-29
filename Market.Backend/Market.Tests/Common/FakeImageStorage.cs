using Market.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Market.Tests.Common;

/// <summary>
/// In-memory <see cref="IImageStorage"/> that records what a handler asked it to do,
/// so tests can assert on uploads/deletes without touching blob storage.
/// </summary>
public sealed class FakeImageStorage : IImageStorage
{
    public const string PublicPrefix = "https://blobs.test/";

    /// <summary>Path handed back for the next successful upload.</summary>
    public string SavedPath { get; set; } = "events/uploaded-image.png";

    public List<string> Saved { get; } = [];
    public List<string> Deleted { get; } = [];

    public Task<string?> SaveAsync(ImageStorageCategory category, IFormFile? image, CancellationToken ct = default)
    {
        if (image is null || image.Length == 0)
            return Task.FromResult<string?>(null);

        Saved.Add(SavedPath);
        return Task.FromResult<string?>(SavedPath);
    }

    public Task DeleteIfExistsAsync(ImageStorageCategory category, string? storedPath, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(storedPath))
            Deleted.Add(storedPath);

        return Task.CompletedTask;
    }

    public async Task<string?> ReplaceIfUploadedAsync(
        ImageStorageCategory category,
        string? currentStoredPath,
        IFormFile? newImage,
        CancellationToken ct = default)
    {
        if (newImage is null || newImage.Length == 0)
            return currentStoredPath;

        await DeleteIfExistsAsync(category, currentStoredPath, ct);
        return await SaveAsync(category, newImage, ct);
    }

    public string? ToPublicPath(ImageStorageCategory category, string? storedPath) =>
        string.IsNullOrWhiteSpace(storedPath) ? null : $"{PublicPrefix}{category}/{storedPath}";
}
