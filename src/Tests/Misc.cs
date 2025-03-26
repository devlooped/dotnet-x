using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Devlooped.Tests;

public class Misc(ITestOutputHelper output)
{
    const string Text = "Hayek was a lion, no doubt.";

    [LocalFact("X:AccessToken", "X:AccessTokenSecret", "X:ConsumerKey", "X:ConsumerSecret")]
    public async Task CreatePost()
    {
        App.Create(out var services);
        using var http = services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var response = await http.PostAsJsonAsync("https://api.twitter.com/2/tweets", new { text = Text });

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            output.WriteLine(content);
        }

        Assert.True(response.IsSuccessStatusCode);
    }

    [LocalFact("X:AccessToken", "X:AccessTokenSecret", "X:ConsumerKey", "X:ConsumerSecret")]
    public async Task UploadMediaAndCreatePost()
    {
        App.Create(out var services);
        using var http = services.GetRequiredService<IHttpClientFactory>().CreateClient();

        // Upload media
        var mediaContent = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(await File.ReadAllBytesAsync("Hayek-Lion.jpg"));
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        mediaContent.Add(imageContent, "media", "image.jpg");

        var mediaResponse = await http.PostAsync("https://upload.twitter.com/1.1/media/upload.json", mediaContent);

        if (!mediaResponse.IsSuccessStatusCode)
        {
            var content = await mediaResponse.Content.ReadAsStringAsync();
            output.WriteLine(content);
        }

        Assert.True(mediaResponse.IsSuccessStatusCode);

        var mediaResult = await mediaResponse.Content.ReadFromJsonAsync<JsonElement>();
        var mediaId = mediaResult.GetProperty("media_id_string").GetString();

        // Create post with media
        var tweetResponse = await http.PostAsJsonAsync("https://api.twitter.com/2/tweets", new
        {
            text = Text,
            media = new { media_ids = new[] { mediaId } }
        });

        if (!tweetResponse.IsSuccessStatusCode)
        {
            var content = await tweetResponse.Content.ReadAsStringAsync();
            output.WriteLine(content);
        }

        Assert.True(tweetResponse.IsSuccessStatusCode);
    }
}
