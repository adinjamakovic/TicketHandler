using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Market.Application.Abstractions;
using Market.Application.Common.Validation;
using Microsoft.AspNetCore.Http;

namespace Market.Application.Common.Services;

public sealed class ImageStorage : IImageStorage
{
    private const string ContainerName = "uploads";
    private readonly BlobServiceClient _client;
    private readonly IImageCompressor _compressor;

    public ImageStorage(BlobServiceClient client, IImageCompressor compressor)
    {
        _client = client;
        _compressor = compressor;
    }

    private static string GetCategoryFolder(ImageStorageCategory category) => category switch
    {
        ImageStorageCategory.Events => "Events",
        ImageStorageCategory.Organizers => "Organizers",
        ImageStorageCategory.Performers => "Performers",
        ImageStorageCategory.EventNews => "EventNews",
        ImageStorageCategory.LocationImages => "Locations",
        ImageStorageCategory.PointsOfSale => "PointsOfSale",
        _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Unknown image storage category.")
    };

    public async Task<string?> SaveAsync(ImageStorageCategory category, IFormFile? image, CancellationToken ct = default)
    {
        if (image is null || image.Length == 0)
            return null;

        var extension = Path.GetExtension(image.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !ImageValidationRules.AllowedExtensions.Contains(extension))
            throw new MarketBusinessRuleException(
                "image.invalid",
                "Image must be a JPG, PNG, WEBP, or GIF file.");

        // Downscale/re-encode first — the container only ever receives web-sized bytes, and the
        // extension/content type come from the compressor since re-encoding can change the format.
        await using var compressed = await _compressor.CompressAsync(image, ct);

        var container = _client.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobName = $"{GetCategoryFolder(category)}/{Guid.NewGuid()}{compressed.Extension}";
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(compressed.Content, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = compressed.ContentType }
        }, ct);

        return blobName;
    }

    public async Task DeleteIfExistsAsync(ImageStorageCategory category, string? storedPath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return;

        var container = _client.GetBlobContainerClient(ContainerName);
        await container.GetBlobClient(storedPath).DeleteIfExistsAsync(cancellationToken: ct);
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

    public string? ToPublicPath(ImageStorageCategory category, string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;

        var container = _client.GetBlobContainerClient(ContainerName);
        return container.GetBlobClient(storedPath).Uri.ToString();
    }
}
