using Market.Application.Abstractions;

namespace Market.Application.Common;

public static class ImageStorageQueryExtensions
{
    public static void ApplyPublicImagePaths<T>(
        this IEnumerable<T> items,
        IImageStorage storage,
        ImageStorageCategory category,
        Func<T, string?> getPath,
        Action<T, string?> setPath)
    {
        foreach (var item in items)
            setPath(item, storage.ToPublicPath(category, getPath(item)));
    }
}
