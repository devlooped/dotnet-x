using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace Devlooped;

public class PostCommand(IHttpClientFactory httpFactory, IAnsiConsole console) : AsyncCommand<PostCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, PostCommandSettings settings)
    {
        using var http = httpFactory.CreateClient();

        var response = await console.Status().StartAsync("Posting...", async ctx =>
        {
            var mediaIds = await UploadMediaAsync(ctx, http, settings.Media);
            ctx.Status("Posting...");
            var response = await http.PostAsJsonAsync("https://api.twitter.com/2/tweets", new
            {
                text = settings.Text,
                media = new { media_ids = mediaIds }
            });
            return response;
        });

        var json = await response.Content.ReadAsStringAsync();
        console.Write(new JsonText(json));

        if (!response.IsSuccessStatusCode)
            return (int)response.StatusCode;

        return 0;
    }

    static async Task<string[]> UploadMediaAsync(StatusContext ctx, HttpClient http, PostCommandSettings.MediaList mediaFiles)
    {
        if (mediaFiles.Count == 0)
            return [];

        var mediaIds = new List<string>();

        foreach (var media in mediaFiles)
        {
            // Should never happen since we validated this via settings
            if (!MediaTypes.TryGetMediaType(media, out var mediaType))
                continue;

            ctx.Status($"Uploading {Path.GetFileName(media)}...");

            var mediaContent = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(await File.ReadAllBytesAsync(media));
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
            mediaContent.Add(imageContent, "media", Path.GetFileName(media));

            var response = await http.PostAsync("https://upload.twitter.com/1.1/media/upload.json", mediaContent);

            await response.EnsureSuccessAsync((string? error) => $"Failed to upload media {media}: {error}");

            var mediaResult = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (mediaResult.GetProperty("media_id_string").GetString() is string mediaId)
                mediaIds.Add(mediaId);
        }

        return [.. mediaIds];
    }
}

public class PostCommandSettings : CommandSettings
{
    [Description("Text to post")]
    [CommandArgument(0, "<TEXT>")]
    public required string Text { get; set; }

    [MediaTypeDescription("One or more media files to attach to the post")]
    [CommandOption("-m|--media <MEDIA>")]
    public MediaList Media { get; set; } = [];

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return ValidationResult.Error("Text is required.");

        Text = Text.Trim();

        foreach (var media in Media)
        {
            if (!File.Exists(media))
                return ValidationResult.Error($"Could not locate media file {media}.");
            if (!MediaTypes.TryGetMediaType(media, out var mediaType))
                return ValidationResult.Error($"Unsupported media type for {media}. Supported types are: {string.Join(", ", MediaTypes.GetSupportedExtensions())}.");
        }

        return base.Validate();
    }

    [TypeConverter(typeof(MediaListConverter))]
    public class MediaList : List<string>
    {
        public MediaList() { }

        public MediaList(string mediaString)
        {
            AddRange(mediaString
                .Split(',', ';')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s)));
        }

        public override string ToString() => string.Join(", ", this);

        class MediaListConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
                => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

            public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
                => destinationType == typeof(MediaList) || base.CanConvertTo(context, destinationType);

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (value is null)
                    return null;

                if (destinationType == typeof(string) && value is string values)
                    return new MediaList(values);

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
