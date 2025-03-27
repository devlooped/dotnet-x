using GitCredentialManager;
using Microsoft.Extensions.Configuration;

namespace Devlooped;

static class AuthStoreExtensions
{
    public static string? GetActive(this ICredentialStore store)
    {
        var current = store.Get("https://api.x.com", "X:ACTIVE");
        return current?.Password;
    }

    public static void SetActive(this IConfiguration configuration, string alias)
        => configuration["X:ACTIVE"] = alias;

    public static string[] RemoveAll(this ICredentialStore store)
    {
        var removed = new List<string>();
        foreach (var alias in store.GetAccounts("https://api.x.com")
            .Where(x => x.StartsWith("X:Alias:")).Select(x => x[8..]))
        {
            if (store.Remove(alias))
                removed.Add(alias);
        }
        return [.. removed];
    }

    public static bool Remove(this ICredentialStore store, string alias)
    {
        var removed = store.Remove("https://api.x.com", $"{alias}:X:ConsumerKey");
        removed |= store.Remove("https://api.x.com", $"{alias}:X:ConsumerSecret");
        removed |= store.Remove("https://api.x.com", $"{alias}:X:AccessToken");
        removed |= store.Remove("https://api.x.com", $"{alias}:X:AccessTokenSecret");
        removed |= store.Remove("https://api.x.com", $"X:Alias:{alias}");
        return removed;
    }

    /// <summary>
    /// Saves the non-null values specified in the options to the credential store.
    /// </summary>
    /// <returns>The new or updated values that were persisted.</returns>
    public static void Save(this ICredentialStore store, string alias, IAuthSettings settings)
    {
        if (settings.ConsumerKey != null)
            store.AddOrUpdate("https://api.x.com", $"{alias}:X:ConsumerKey", settings.ConsumerKey);
        if (settings.ConsumerSecret != null)
            store.AddOrUpdate("https://api.x.com", $"{alias}:X:ConsumerSecret", settings.ConsumerSecret);
        if (settings.AccessToken != null)
            store.AddOrUpdate("https://api.x.com", $"{alias}:X:AccessToken", settings.AccessToken);
        if (settings.AccessTokenSecret != null)
            store.AddOrUpdate("https://api.x.com", $"{alias}:X:AccessTokenSecret", settings.AccessTokenSecret);

        store.AddOrUpdate("https://api.x.com", $"X:Alias:{alias}", alias);
    }

    public static IAuthSettings? Read(this ICredentialStore store, string alias)
    {
        var settings = Activator.CreateInstance<AuthOptions>();

        if (store.Get("https://api.x.com", $"{alias}:X:AccessToken") is { Password: { Length: > 0 } accessToken })
            settings.AccessToken = accessToken;
        if (store.Get("https://api.x.com", $"{alias}:X:AccessTokenSecret") is { Password: { Length: > 0 } accessTokenSecret })
            settings.AccessTokenSecret = accessTokenSecret;
        if (store.Get("https://api.x.com", $"{alias}:X:ConsumerKey") is { Password: { Length: > 0 } consumerKey })
            settings.ConsumerKey = consumerKey;
        if (store.Get("https://api.x.com", $"{alias}:X:ConsumerSecret") is { Password: { Length: > 0 } consumerSecret })
            settings.ConsumerSecret = consumerSecret;

        if (settings.AccessToken == null &&
            settings.AccessTokenSecret == null &&
            settings.ConsumerKey == null &&
            settings.ConsumerSecret == null)
            return null;

        return settings;
    }
}
