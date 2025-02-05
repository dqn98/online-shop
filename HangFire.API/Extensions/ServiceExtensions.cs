using Shared.Configurations;

namespace HangFire.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var hangFireSettings = configuration.GetSection(nameof(HangFireSettings)).Get<HangFireSettings>();
        ArgumentNullException.ThrowIfNull(hangFireSettings, nameof(hangFireSettings));
        
        services.AddSingleton(hangFireSettings);

        return services;
    }
}