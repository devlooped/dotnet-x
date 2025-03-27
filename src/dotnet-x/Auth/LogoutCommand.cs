using System.ComponentModel;
using System.Dynamic;
using GitCredentialManager;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Devlooped.Auth;

[Description("Log out of X")]
public class LogoutCommand(IAnsiConsole console, ICredentialStore store) : Command<LogoutCommand.LogoutSettings>
{
    public override int Execute(CommandContext context, LogoutSettings settings)
    {
        if (settings.Alias != null)
        {
            if (store.Remove(settings.Alias))
                console.MarkupLine($"  :check_mark_button: Logged out {settings.Alias}");
            else
                console.MarkupLine($"  :white_question_mark: No credentials found for {settings.Alias}");
        }
        else
        {
            if (store.RemoveAll() is { Length: > 0 } removed)
                console.MarkupLine($"  :check_mark_button: Logged out {string.Join(", ", removed)}");
            else
                console.MarkupLine($"  :white_question_mark: No accounts found to log out");
        }

        return 0;
    }

    public class LogoutSettings : CommandSettings
    {
        [Description("Specific alias to log out. Removes all accounts if not provided")]
        [CommandArgument(0, "[alias]")]
        public string? Alias { get; set; }
    }
}
