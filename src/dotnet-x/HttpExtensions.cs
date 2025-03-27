using System.Net.Http.Json;
using System.Text.Json;

namespace Devlooped;

static class HttpExtensions
{
    public static async Task EnsureSuccessAsync(this HttpResponseMessage response, Func<string?, string> formatError)
    {
        if (response.IsSuccessStatusCode)
            return;

        var jsonResponse = await response.Content.ReadAsStringAsync();

        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (errorResponse.TryGetProperty("error", out var error) && error.GetString() is string errorMessage)
                throw new Exception(formatError(errorMessage));
            else
                throw new Exception(formatError(null));
        }
        catch (JsonException ex)
        {
            throw new Exception(formatError(ex.Message));
        }
    }
}
