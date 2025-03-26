using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spectre.Console.Cli;

namespace Devlooped;

public static class App
{
    static readonly CancellationTokenSource shutdownCancellation = new();

    static App() => Console.CancelKeyPress += (s, e) => shutdownCancellation.Cancel();

    public static CommandApp Create() => Create(out _);

    public static CommandApp Create(out IServiceProvider services)
    {
        var collection = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<TypeRegistrar>()
            .AddDotNetConfig()
            .Build();

        collection.AddSingleton(configuration)
            .AddSingleton<IConfiguration>(_ => configuration)
            .Configure<AuthOptions>(configuration.GetSection("X"))
            .AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidation>()
            .AddTransient<AuthMessageHandler>()
            .AddSingleton(shutdownCancellation)
            .AddSingleton(JsonOptions.Default);

        collection.AddHttpClient()
            .ConfigureHttpClientDefaults(defaults =>
            {
                defaults.ConfigureHttpClient(http =>
                {
                    http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(ThisAssembly.Info.Product, ThisAssembly.Info.InformationalVersion));

                    if (Debugger.IsAttached)
                        http.Timeout = TimeSpan.FromMinutes(10);
                })
                .AddHttpMessageHandler<AuthMessageHandler>()
                .AddStandardResilienceHandler();
            });

        var registrar = new TypeRegistrar(collection);
        var app = new CommandApp(registrar);
        registrar.Services.AddSingleton<ICommandApp>(app);

        app.Configure(config =>
        {
            config.SetApplicationName(ThisAssembly.Project.ToolCommandName);
            config.AddCommand<PostCommand>("post");
            if (Environment.GetEnvironmentVariables().Contains("NO_COLOR"))
                config.Settings.HelpProviderStyles = null;
        });

        services = registrar.Services.BuildServiceProvider();

        return app;
    }
}
