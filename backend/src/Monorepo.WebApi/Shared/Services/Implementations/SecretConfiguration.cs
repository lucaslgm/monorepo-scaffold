using System.Diagnostics.CodeAnalysis;

namespace Monorepo.WebApi.Shared.Services.Implementations;

[ExcludeFromCodeCoverage]
public static class SecretConfiguration
{
    public static string GetSecretConfigurationValue(string key, IConfiguration configuration)
    {
        var fromEnv = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        var appsettingsJsonKey = key.Replace("__", ":");
        var fromConfig = configuration.GetValue<string>(appsettingsJsonKey);

        return fromConfig ?? string.Empty;
    }
}
