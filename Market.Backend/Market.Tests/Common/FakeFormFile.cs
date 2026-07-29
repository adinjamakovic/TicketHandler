using Microsoft.AspNetCore.Http;

namespace Market.Tests.Common;

/// <summary>
/// Minimal <see cref="IFormFile"/> for multipart commands — only the members the
/// validators and image storage actually read (<see cref="FileName"/> and <see cref="Length"/>).
/// </summary>
public sealed class FakeFormFile : IFormFile
{
    private readonly byte[] _content;

    public FakeFormFile(string fileName = "poster.png", long length = 1024, string contentType = "image/png")
    {
        FileName = fileName;
        Length = length;
        ContentType = contentType;
        _content = new byte[Math.Min(length, 1024)];
    }

    public string ContentType { get; }
    public string ContentDisposition => $"form-data; name=\"{Name}\"; filename=\"{FileName}\"";
    public IHeaderDictionary Headers { get; } = new HeaderDictionary();
    public long Length { get; }
    public string Name => "Image";
    public string FileName { get; }

    public void CopyTo(Stream target) => target.Write(_content, 0, _content.Length);

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
        target.WriteAsync(_content, 0, _content.Length, cancellationToken);

    public Stream OpenReadStream() => new MemoryStream(_content);
}
