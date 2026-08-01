using Market.Application.Abstractions;
using Market.Application.Common.Exceptions;
using Market.Shared.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Market.Application.Common.Services;


// ImageSharp-based compressor: downscales oversized uploads, strips metadata and re-encodes to
// WebP (or JPEG) before the bytes ever reach blob storage.
public sealed class ImageCompressor : IImageCompressor
{
    private readonly ImageCompressionOptions _options;

    public ImageCompressor(IOptions<ImageCompressionOptions> options)
    {
        _options = options.Value;
    }

    private string TargetExtension => _options.Format is ImageCompressionFormat.Jpeg ? ".jpg" : ".webp";

    private string TargetContentType => _options.Format is ImageCompressionFormat.Jpeg ? "image/jpeg" : "image/webp";

    public async Task<CompressedImage> CompressAsync(IFormFile image, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(image);

        // Buffer the upload once: IFormFile streams are forward-only, and the original bytes are
        // still needed whenever re-encoding turns out not to be worth it.
        var original = new MemoryStream();

        try
        {
            await using (var upload = image.OpenReadStream())
                await upload.CopyToAsync(original, ct);

            original.Position = 0;

            ImageInfo info;
            try
            {
                info = await Image.IdentifyAsync(original, ct);
            }
            catch (ImageFormatException ex)
            {
                // Unknown or corrupt content. The extension said image, the bytes disagree.
                throw new MarketBusinessRuleException(
                    "image.invalid",
                    "Image could not be read. Upload a valid JPG, PNG, WEBP, or GIF file.",
                    ex);
            }

            original.Position = 0;
            var format = info.Metadata.DecodedImageFormat;

            // Decompression-bomb guard: reject on the header, before allocating the bitmap.
            if ((long)info.Width * info.Height > _options.MaxPixels)
                throw new MarketBusinessRuleException(
                    "image.resolution_too_large",
                    $"Image resolution must be {_options.MaxPixels / 1_000_000d:0.#} megapixels or smaller.");

            using var decoded = await Image.LoadAsync(original, ct);
            original.Position = 0;

            decoded.Mutate(ctx => ctx.AutoOrient());

            if (decoded.Width > _options.MaxWidth || decoded.Height > _options.MaxHeight)
                decoded.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(_options.MaxWidth, _options.MaxHeight)
                }));

            if (_options.Format is ImageCompressionFormat.Jpeg)
                decoded.Mutate(ctx => ctx.BackgroundColor(Color.White)); // JPEG has no alpha channel

            StripMetadata(decoded);

            var encoded = new MemoryStream();
            await EncodeAsync(decoded, encoded, ct);

            if (encoded.Length >= original.Length)
            {
                await encoded.DisposeAsync();
                return Passthrough(original, format, image);
            }

            encoded.Position = 0;
            await original.DisposeAsync();

            return new CompressedImage(encoded, TargetContentType, TargetExtension);
        }
        catch
        {
            await original.DisposeAsync();
            throw;
        }
    }

    private Task EncodeAsync(Image image, Stream destination, CancellationToken ct) =>
        _options.Format switch
        {
            ImageCompressionFormat.Jpeg => image.SaveAsJpegAsync(
                destination,
                new JpegEncoder { Quality = _options.Quality },
                ct),
            _ => image.SaveAsWebpAsync(
                destination,
                new WebpEncoder { Quality = _options.Quality, FileFormat = WebpFileFormatType.Lossy },
                ct)
        };

    private static void StripMetadata(Image image)
    {
        image.Metadata.ExifProfile = null;
        image.Metadata.IccProfile = null;
        image.Metadata.IptcProfile = null;
        image.Metadata.XmpProfile = null;
    }

    private static CompressedImage Passthrough(MemoryStream original, IImageFormat? format, IFormFile image)
    {
        original.Position = 0;

        var extension = format?.FileExtensions.FirstOrDefault() is { } detected
            ? $".{detected}"
            : Path.GetExtension(image.FileName);

        return new CompressedImage(original, format?.DefaultMimeType ?? image.ContentType, extension);
    }
}
