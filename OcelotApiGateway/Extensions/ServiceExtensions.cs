using Ocelot.DependencyInjection;
using Shared.Configurations;

namespace OcelotApiGateway.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
    
    public static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration);
    }
    
    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("AllowedOrigins").Get<string>();
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(origins ?? string.Empty)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }
}