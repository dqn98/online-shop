namespace OcelotApiGateway.Extensions;

public static class HostExtensions
{
    public static void AddAppConfigurations(this ConfigurationManager configuration)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        configuration.AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddJsonFile($"ocelot.{env}.json", false, true)
            .AddEnvironmentVariables();
    }
}