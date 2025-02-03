using Hangfire;
using Shared.Configurations;

namespace HangFire.API.Extensions;

public static class HostExtensions
{
    public static void AddAppConfigurations(this ConfigurationManager configuration)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        configuration.AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddEnvironmentVariables();
    }

    internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var configurationDashboard = configuration.GetSection("HangFireSettings:Dashboard").Get<DashboardOptions>();
        var hangfireSettings = configuration.GetSection("HangFireSettings").Get<HangFireSettings>();
        ArgumentNullException.ThrowIfNull(configurationDashboard);
        ArgumentNullException.ThrowIfNull(hangfireSettings);
        
        var hangfireRoute = hangfireSettings.Route;

        app.UseHangfireDashboard(hangfireRoute, new DashboardOptions()
        {
            // Authorization = new []{},
            DashboardTitle = configurationDashboard.DashboardTitle,
            StatsPollingInterval = configurationDashboard.StatsPollingInterval,
            AppPath = configurationDashboard.AppPath,
            IgnoreAntiforgeryToken = true,
        });
        return app;
    }
}