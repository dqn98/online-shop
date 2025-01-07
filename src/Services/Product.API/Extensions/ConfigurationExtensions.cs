namespace Product.API.Extensions;

public static class ConfigurationExtensions
{
    public static void AddAppConfigurations(this ConfigurationManager configuration)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        configuration.AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddEnvironmentVariables();
    }
}