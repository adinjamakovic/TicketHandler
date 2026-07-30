using Microsoft.AspNetCore.Http;

namespace Market.Application.Common.Validation;

/// <summary>
/// Shared upload rules for the optional image/logo properties carried by multipart commands.
/// </summary>
public static class ImageValidationRules
{
    public const long MaxImageSizeBytes = 5 * 1024 * 1024;

    public static readonly IReadOnlySet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    /// <summary>
    /// Rejects unsupported file types and oversized uploads. A missing/empty file is valid — images are optional.
    /// </summary>
    public static IRuleBuilderOptions<T, IFormFile?> ValidImage<T>(this IRuleBuilder<T, IFormFile?> rule) =>
        rule
            .Must(HasAllowedExtension)
                .WithMessage($"Image must be one of the following types: {string.Join(", ", AllowedExtensions)}.")
            .Must(file => file is null || file.Length <= MaxImageSizeBytes)
                .WithMessage($"Image must be {MaxImageSizeBytes / (1024 * 1024)} MB or smaller.");

    private static bool HasAllowedExtension(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName);
        return !string.IsNullOrWhiteSpace(extension) && AllowedExtensions.Contains(extension);
    }
}
