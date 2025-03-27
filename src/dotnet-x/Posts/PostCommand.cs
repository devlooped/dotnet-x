using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Devlooped.Posts;

public class PostCommand(IHttpClientFactory httpFactory, IAnsiConsole console) : AsyncCommand<PostCommandSettings>
{
    const string MediaEndpointUrl = "https://upload.twitter.com/1.1/media/upload.json";
    const int ChunkSize = 4 * 1024 * 1024; // 4MB chunk size

    public override async Task<int> ExecuteAsync(CommandContext context, PostCommandSettings settings)
    {
        using var http = httpFactory.CreateClient();

        var response = await console.Status().StartAsync("Posting...", async ctx =>
        {
            var mediaIds = await UploadMediaAsync(ctx, http, settings.Media);
            ctx.Status("Posting...");

            object body = mediaIds.Length == 0 ?
                new { text = settings.Text } : 
                new { text = settings.Text, media = new { media_ids = mediaIds } };

            var response = await http.PostAsJsonAsync("https://api.twitter.com/2/tweets", body);
            return response;
        });

        var json = await response.Content.ReadAsStringAsync();

        if (settings.JQ.IsSet || settings.Json)
            return console.RenderJson(json, settings);

        var id = await JQ.ExecuteAsync(json, ".data.id");
        if (id is { Length: > 0 })
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
                console.MarkupLine($"  :check_mark_button: Posted at [link=https://x.com/i/status/{id}]{id}[/]");
            else
                console.WriteLine($"Posted: https://x.com/i/status/{id}");
        }
        else
        {
            settings.Json = true;
            console.RenderJson(json, "");
        }

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

            // For larger files, use chunked upload
            if (new FileInfo(media).Length > ChunkSize)
            {
                var upload = new MediaUploader(ctx, media, mediaType);
                var mediaId = await upload.UploadAsync(http);
                if (mediaId is not null)
                    mediaIds.Add(mediaId);
            }
            else
            {
                // Small images use the simple upload method
                var mediaContent = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(await File.ReadAllBytesAsync(media));
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
                mediaContent.Add(imageContent, "media", Path.GetFileName(media));

                var response = await http.PostAsync(MediaEndpointUrl, mediaContent);
                await response.EnsureSuccessAsync((string? error) => $"Failed to upload media {media}: {error}");

                var mediaResult = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (mediaResult.GetProperty("media_id_string").GetString() is string mediaId)
                    mediaIds.Add(mediaId);
            }
        }

        return [.. mediaIds];
    }

    class MediaUploader(StatusContext ctx, string filePath, string mediaType)
    {
        readonly long totalBytes = new FileInfo(filePath).Length;
        readonly string mediaCategory = mediaType.StartsWith("video/") ? "tweet_video" : "tweet_image";
        readonly string fileName = Path.GetFileName(filePath);
        string? mediaId;
        JsonElement? processingInfo;

        public async Task<string?> UploadAsync(HttpClient http)
        {
            await InitializeUploadAsync(http);
            if (mediaId is null)
                return null;
                
            await AppendChunksAsync(http);
            await FinalizeUploadAsync(http);
            await CheckStatusAsync(http);
            
            return mediaId;
        }

        async Task InitializeUploadAsync(HttpClient http)
        {
            ctx.Status($"Initializing upload for {fileName}...");
            
            var requestData = new Dictionary<string, string>
            {
                ["command"] = "INIT",
                ["media_type"] = mediaType,
                ["total_bytes"] = totalBytes.ToString(),
                //["media_category"] = mediaCategory
            };
            
            //var response = await http.PostAsync($"{MediaEndpointUrl}?{string.Join("&", requestData.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}", null);
            var response = await http.PostAsync(MediaEndpointUrl, new FormUrlEncodedContent(requestData));
            await response.EnsureSuccessAsync((string? error) => $"Failed to initialize media upload for {fileName}: {error}");
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            mediaId = result.GetProperty("media_id_string").GetString();
            
            ctx.Status($"Media ID: {mediaId}");
        }

        async Task AppendChunksAsync(HttpClient http) 
        {
            int segmentId = 0;
            long bytesSent = 0;

            using var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var buffer = new byte[ChunkSize];
            int bytesRead;
            
            while ((bytesRead = await file.ReadAsync(buffer)) > 0)
            {
                var percentComplete = (double)bytesSent / totalBytes * 100;
                ctx.Status($"Uploading {fileName}... ({percentComplete:0.0}% complete)");
                
                bytesSent += bytesRead;
                
                var content = new MultipartFormDataContent
                {
                    { new StringContent("APPEND"), "command" },
                    { new StringContent(mediaId!), "media_id" },
                    { new StringContent(segmentId.ToString()), "segment_index" }
                };
                
                var mediaChunk = new ByteArrayContent(buffer, 0, bytesRead);
                mediaChunk.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                content.Add(mediaChunk, "media");
                
                var response = await http.PostAsync(MediaEndpointUrl, content);
                await response.EnsureSuccessAsync((string? error) => $"Failed to upload {fileName}: {error}");
                
                segmentId++;
            }
        }

        async Task FinalizeUploadAsync(HttpClient http)
        {
            ctx.Status("Finalizing upload...");
            
            var requestData = new Dictionary<string, string>
            {
                ["command"] = "FINALIZE",
                ["media_id"] = mediaId!
            };
            
            var response = await http.PostAsync($"{MediaEndpointUrl}?{string.Join("&", requestData.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}", null);
            await response.EnsureSuccessAsync((string? error) => $"Failed to finalize upload for {fileName}: {error}");
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (result.TryGetProperty("processing_info", out var info))
                processingInfo = info;
        }

        async Task CheckStatusAsync(HttpClient http, int retryCount = 0)
        {
            if (processingInfo is null)
                return;
                
            var state = processingInfo.Value.GetProperty("state").GetString();
            ctx.Status($"Media processing status: {state}");
            
            if (string.Equals(state, "succeeded", StringComparison.OrdinalIgnoreCase))
                return;
                
            if (string.Equals(state, "failed", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Media processing failed");
            
            if (retryCount > 10) // Limit retries to avoid infinite loops
                throw new Exception("Media processing timeout");
                
            if (processingInfo.Value.TryGetProperty("check_after_secs", out var checkAfterProperty))
            {
                var checkAfterSecs = checkAfterProperty.GetInt32();
                ctx.Status($"Waiting {checkAfterSecs} seconds before checking status again...");
                await Task.Delay(TimeSpan.FromSeconds(checkAfterSecs));
                
                var requestParams = new Dictionary<string, string>
                {
                    ["command"] = "STATUS",
                    ["media_id"] = mediaId!
                };
                
                var response = await http.GetAsync($"{MediaEndpointUrl}?{string.Join("&", requestParams.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"))}");
                await response.EnsureSuccessAsync((string? error) => $"Failed to check media status for {fileName}: {error}");
                
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (result.TryGetProperty("processing_info", out var info))
                {
                    processingInfo = info;
                    await CheckStatusAsync(http, retryCount + 1);
                }
            }
        }
    }
}

public class PostCommandSettings : JsonCommandSettings
{
    [Description("Text to post")]
    [CommandArgument(0, "<TEXT>")]
    public required string Text { get; set; }

    [MediaTypeDescription("Zero or more media files to attach to the post")]
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
