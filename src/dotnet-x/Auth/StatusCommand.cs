using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Devlooped.Auth;

[Description("Shows the current authentication status")]
class StatusCommand(IAnsiConsole console, IHttpClientFactory httpFactory, IOptionsMonitor<AuthOptions> optionsMonitor) : AsyncCommand<StatusCommand.StatusSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, StatusSettings settings)
    {
        var options = optionsMonitor.CurrentValue;

        var user = await console.Status().StartAsync("Reading user info...", async ctx =>
        {
            using var http = httpFactory.CreateClient();
            var response = await http.GetAsync("https://api.twitter.com/2/users/me");
            await response.EnsureSuccessAsync(error => $"Failed to authenticate: {error}");

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("data").GetProperty("username").GetString();
        });

        console.MarkupLine($"  :check_mark_button: Logged in as {user}");
        if (settings.ShowSecrets)
        {
            var table = new Table()
                .Border(TableBorder.None)
                .AddColumn(string.Empty)
                .AddColumn(string.Empty);

            table.AddRow("[grey]ConsumerKey[/]", options.ConsumerKey);
            table.AddRow("[grey]ConsumerSecret[/]", options.ConsumerSecret);
            table.AddRow("[grey]AccessToken[/]", options.AccessToken);
            table.AddRow("[grey]AccessTokenSecret[/]", options.AccessTokenSecret);

            console.Write(new Padder(table, new Padding(5, 0, 0, 0)));
        }

        return 0;
    }

    public class StatusSettings : CommandSettings
    {
        [Description("Display the secrets")]
        [DefaultValue(false)]
        [CommandOption("--show-secrets")]
        public bool ShowSecrets { get; set; }
    }
}
