using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Devlooped;

public class AuthOptions
{
    public required string ConsumerKey { get; set; }
    public required string ConsumerSecret { get; set; }
    public required string AccessToken { get; set; }
    public required string AccessTokenSecret { get; set; }
}

public class AuthOptionsValidation(IConfiguration configuration) : IValidateOptions<AuthOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthOptions options)
    {
        if (!configuration.GetSection("X").Exists())
            return ValidateOptionsResult.Fail("Configuration section 'X' is missing.");

        return Validate(options);
    }

    public static ValidateOptionsResult Validate(AuthOptions options)
    {
        if (options.ConsumerKey == null)
            return ValidateOptionsResult.Fail($"Missing X:ConsumerKey configuration");
        if (options.ConsumerSecret == null)
            return ValidateOptionsResult.Fail($"Missing X:ConsumerSecret configuration");
        if (options.AccessToken == null)
            return ValidateOptionsResult.Fail($"Missing X:AccessToken configuration");
        if (options.AccessTokenSecret == null)
            return ValidateOptionsResult.Fail($"Missing X:AccessTokenSecret configuration");

        return ValidateOptionsResult.Success;
    }
}