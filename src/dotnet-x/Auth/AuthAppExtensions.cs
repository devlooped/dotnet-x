using Devlooped.Auth;
using Spectre.Console.Cli;

namespace Devlooped;

static class AuthAppExtensions
{
    public static ICommandApp UseAuth(this ICommandApp app)
    {
        app.Configure(config =>
        {
            config.AddBranch("auth", group =>
            {
                group.AddCommand<LoginCommand>("login");
                group.AddCommand<ListCommand>("list").IsHidden();
                group.AddCommand<LogoutCommand>("logout");
                group.AddCommand<StatusCommand>("status");
            });
        });
        return app;
    }
}
