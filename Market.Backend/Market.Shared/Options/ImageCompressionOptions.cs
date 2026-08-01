using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options;

// "ImageCompression" section, how uploads are downscaled/re-encoded before hitting blob storage
public sealed class ImageCompressionOptions
{
    public const string SectionName = "ImageCompression";

    // When false, uploads are stored exactly as received.
    public bool Enabled { get; init; } = true;

    // Longest allowed width; larger images are scaled down, aspect ratio preserved.
    [Range(16, 10000)] public int MaxWidth { get; init; } = 1920;

    // Longest allowed height; larger images are scaled down, aspect ratio preserved.
    [Range(16, 10000)] public int MaxHeight { get; init; } = 1920;
    [Range(1, 100)] public int Quality { get; init; } = 80;

    // Output encoding. WebP keeps transparency; JPEG is flattened onto white.
    public ImageCompressionFormat Format { get; init; } = ImageCompressionFormat.Webp;
    [Range(1, 500_000_000)] public long MaxPixels { get; init; } = 50_000_000;
}

public enum ImageCompressionFormat
{
    Webp = 0,
    Jpeg = 1
}
