using Market.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Market.Application.Common.Services;

public sealed class ImageStorage(IWebHostEnvironment env) : IImageStorage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    public static string GetRelativeFolder(ImageStorageCategory category) => category switch
    {
        ImageStorageCategory.Events => "Upload/Events",
        ImageStorageCategory.Organizers => "Upload/Organizers",
        ImageStorageCategory.Performers => "Upload/Performers",
        ImageStorageCategory.EventNews => "Upload/EventNews",
        ImageStorageCategory.LocationImages => "Upload/Locations",
        ImageStorageCategory.PointsOfSale => "Upload/PointsOfSale",
        _ => throw new ArgumentOutOfRangeException(nameof(category), category, "Unknown image storage category.")
    };

    public async Task<string?> SaveAsync(ImageStorageCategory category, IFormFile? image, CancellationToken ct = default)
    {
        if (image is null || image.Length == 0)
            return null;

        var extension = Path.GetExtension(image.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new MarketBusinessRuleException(
                "image.invalid",
                "Image must be a JPG, PNG, WEBP, or GIF file.");

        var relativeFolder = GetRelativeFolder(category);
        var uploadDirectory = Path.Combine(env.WebRootPath ?? string.Empty, relativeFolder);
        Directory.CreateDirectory(uploadDirectory);

        var fileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var physicalPath = Path.Combine(uploadDirectory, fileName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
            await image.CopyToAsync(stream, ct);

        return $"{relativeFolder}/{fileName}";
    }

    public void DeleteIfExists(ImageStorageCategory category, string? storedPath)
    {
        var physicalPath = ResolvePhysicalPath(storedPath);
        if (physicalPath is not null && File.Exists(physicalPath))
            File.Delete(physicalPath);
    }

    public async Task<string?> ReplaceIfUploadedAsync(
        ImageStorageCategory category,
        string? currentStoredPath,
        IFormFile? newImage,
        CancellationToken ct = default)
    {
        if (newImage is null || newImage.Length == 0)
            return currentStoredPath;

        DeleteIfExists(category, currentStoredPath);
        return await SaveAsync(category, newImage, ct);
    }

    public string? ToPublicPath(ImageStorageCategory category, string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;

        var relativeFolder = GetRelativeFolder(category);

        if (storedPath.StartsWith('/'))
            return storedPath.Replace('\\', '/');

        if (Path.IsPathRooted(storedPath))
        {
            var fileName = Path.GetFileName(storedPath);
            return string.IsNullOrEmpty(fileName) ? null : $"/{relativeFolder}/{fileName}";
        }

        var normalized = storedPath.Replace('\\', '/').TrimStart('/');
        if (!normalized.Contains('/'))
            return $"/{relativeFolder}/{normalized}";

        return "/" + normalized;
    }

    private string? ResolvePhysicalPath(string? storedPath)
    {
        if (string.IsNullOrWhiteSpace(storedPath))
            return null;

        if(Path.IsPathRooted(storedPath))
            return storedPath;

        var relative = storedPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(env.WebRootPath ?? string.Empty, relative);
    }
}
