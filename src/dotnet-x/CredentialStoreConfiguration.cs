using GitCredentialManager;
using Microsoft.Extensions.Configuration;

namespace Devlooped;

public static class CredentialStoreConfigurationExtensions
{
    public static IConfigurationBuilder AddCredentialStore(this IConfigurationBuilder builder, ICredentialStore store)
    {
        builder.Add(new CredentialStoreConfigurationSource(store));
        return builder;
    }

    class CredentialStoreConfigurationSource(ICredentialStore store) : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new CredentialStoreConfigurationProvider(store);
    }

    class CredentialStoreConfigurationProvider(ICredentialStore store) : ConfigurationProvider
    {
        public override void Load() => ReloadData();

        public void Reload()
        {
            Load();
            OnReload();
        }

        public override void Set(string key, string? value)
        {
            if (!key.Contains("X:"))
                return;

            if (value == null)
                store.Remove("https://api.x.com", key);
            else
                store.AddOrUpdate("https://api.x.com", key, value);

            if (key == "X:ACTIVE")
                Reload();
        }

        void ReloadData()
        {
            var active = store.GetActive();
            if (active != null)
            {
                var prefix = active + ":";
                var secrets = store
                    .GetAccounts("https://api.x.com")
                    .Where(x => x.StartsWith(prefix))
                    .Select(x => store.Get("https://api.x.com", x))
                    .ToDictionary(x => x.Account[prefix.Length..], x => (string?)x.Password);

                secrets["X:ACTIVE"] = active;
                Data = secrets;
            }
        }
    }
}