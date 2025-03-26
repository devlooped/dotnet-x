using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Devlooped;

public static class MediaTypes
{
    // MIME type mapping for X's upload.json endpoint
    static readonly Dictionary<string, string> knownMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" }, // Used for both static and animated GIFs
        { ".webp", "image/webp" },

        // Videos
        { ".mp4", "video/mp4" },
        { ".mov", "video/quicktime" }
    };

    public static bool TryGetMediaType(string filePath, [NotNullWhen(true)] out string? mediaType)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            mediaType = null;
            return false;
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return knownMediaTypes.TryGetValue(extension, out mediaType);
    }

    public static IEnumerable<string> GetSupportedExtensions() => knownMediaTypes.Keys;
}

public class MediaTypeDescriptionAttribute(string description, bool parenthesize = true)
    : DescriptionAttribute(GetDescription(description, parenthesize))
{
    static string GetDescription(string description, bool parenthesize) => description.Trim() + " " +
        (parenthesize ? $"({string.Join(", ", MediaTypes.GetSupportedExtensions())})" : string.Join(", ", MediaTypes.GetSupportedExtensions()));
}
