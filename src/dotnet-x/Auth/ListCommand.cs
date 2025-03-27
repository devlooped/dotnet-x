using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Devlooped.Auth;

[Description("Shows the current X configuration values")]
class ListCommand(IAnsiConsole console, IConfiguration configuration) : Command
{
    public override int Execute(CommandContext context)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .AddColumn("Key")
            .AddColumn("Value");

        foreach (var entry in configuration.AsEnumerable().Where(x => x.Key.StartsWith("X:") && x.Value != null))
        {
            table.AddRow($"[grey]{entry.Key}[/]", entry.Value ?? "");
        }

        console.Write(new Padder(table, new Padding(5, 0, 0, 0)));

        return 0;
    }
}
