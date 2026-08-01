using Microsoft.AspNetCore.Http;

namespace Market.Application.Abstractions;
// Downscales and re-encodes an uploaded image so blob storage only ever receives web-sized bytes.
public interface IImageCompressor
{
    Task<CompressedImage> CompressAsync(IFormFile image, CancellationToken ct = default);
}
public sealed class CompressedImage : IAsyncDisposable
{
    public CompressedImage(Stream content, string contentType, string extension)
    {
        Content = content;
        ContentType = contentType;
        Extension = extension;
    }

    public Stream Content { get; }

    public string ContentType { get; }

    public string Extension { get; }

    public ValueTask DisposeAsync() => Content.DisposeAsync();
}
