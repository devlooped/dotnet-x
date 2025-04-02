using System.ComponentModel;
using GitCredentialManager;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Devlooped.Auth;

[Description(
    $$$"""
    Authenticate to X by providing the required secrets. 
    
    Supports API key autentication using the Git Credential Manager for storage.

    Switch easily between keys by just specifying an alias for the keys.

    Alternatively, {{{ThisAssembly.Project.ToolCommandName}}} will use the secrets found in environment variables with the prefix `X_`: `X_AccessToken`, `X_AccessTokenSecret`, `X_ConsumerKey`, `X_ConsumerSecret`.
    Using double underscores also works for nested configuration, such as `X__ConsumerKey`.
    This method is most suitable for "headless" use such as in automation.

    For example, to use {{{ThisAssembly.Project.ToolCommandName}}} in GitHub Actions:
      - name: ✖️ post
        env:
          X_AccessToken: ${{ secrets.X_ACCESS_TOKEN }}
          X_AccessTokenSecret: ${{ secrets.X_ACCESS_TOKEN_SECRET }}
          X_ConsumerKey: ${{ secrets.X_CONSUMER_KEY }}
          X_ConsumerSecret: ${{ secrets.X_CONSUMER_SECRET }}
        run: |
          dotnet tool update -g dotnet-x
          x post "Hello, world!" --media image.png
    """)]
public class LoginCommand(ICommandApp app, ICredentialStore store, IConfiguration configuration) : AsyncCommand<LoginCommand.LoginSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, LoginSettings settings)
    {
        store.Save(settings.Alias, settings);
        configuration.SetActive(settings.Alias);
        return await app.RunAsync(["auth", "status"]);
    }

    public class LoginSettings(ICredentialStore store) : CommandSettings, IAuthSettings
    {
        [Description("Alias to use for the set of credentials")]
        [CommandArgument(0, "<alias>")]
        public required string Alias { get; set; }

        [Description("Access token. Required unless previously saved.")]
        [CommandOption("--at|--access-token")]
        public required string AccessToken { get; set; }

        [Description("Access token secret. Required unless previously saved.")]
        [CommandOption("--ats|--access-token-secret")]
        public required string AccessTokenSecret { get; set; }

        [Description("Consumer key. Required unless previously saved.")]
        [CommandOption("--ck|--consumer-key")]
        public required string ConsumerKey { get; set; }

        [Description("Consumer secret. Required unless previously saved.")]
        [CommandOption("--cs|--consumer-secret")]
        public required string ConsumerSecret { get; set; }

        public override ValidationResult Validate()
        {
            var stored = store.Read(Alias);
            var required = new List<string>();

            if (string.IsNullOrEmpty(AccessToken))
            {
                if (stored?.AccessToken is { })
                    AccessToken = stored.AccessToken;
                else
                    required.Add("--access-token");
            }

            if (string.IsNullOrEmpty(AccessTokenSecret))
            {
                if (stored?.AccessTokenSecret is { })
                    AccessTokenSecret = stored.AccessTokenSecret;
                else
                    required.Add("--access-token-secret");
            }

            if (string.IsNullOrEmpty(ConsumerKey))
            {
                if (stored?.ConsumerKey is { })
                    ConsumerKey = stored.ConsumerKey;
                else
                    required.Add("--consumer-key");
            }

            if (string.IsNullOrEmpty(ConsumerSecret))
            {
                if (stored?.ConsumerSecret is { })
                    ConsumerSecret = stored.ConsumerSecret;
                else
                    required.Add("--consumer-secret");
            }

            if (required.Count > 0)
                return ValidationResult.Error($"No saved credentials exist for {Alias}. Please specify {string.Join(", ", required)}.");

            return base.Validate();
        }
    }
}
