using Devlooped.Posts;
using Spectre.Console.Cli;

namespace Devlooped;

static class PostsAppExtensions
{
    public static ICommandApp UsePosts(this ICommandApp app)
    {
        app.Configure(config =>
        {
            config.AddCommand<PostCommand>("post")
                  .WithExample("post", "\"Hello, world!\"", "--media", "path/to/image.png");
        });
        return app;
    }
}
