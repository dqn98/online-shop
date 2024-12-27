using System.Reflection;
using Infrastructure.Extensions;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;
using MongoDB.Driver;

namespace Inventory.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
            .Get<DatabaseSettings>();
        
        ArgumentNullException.ThrowIfNull(databaseSettings);
        services.AddSingleton(databaseSettings);
        
        return services;
    }

    private static string GetMongoConnectionString(this IServiceCollection services)
    {
        var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        
        ArgumentNullException.ThrowIfNull(settings);
        if(string.IsNullOrEmpty(settings.ConnectionString)) 
            throw new ArgumentNullException($"Database setting is not configured - {nameof(settings.ConnectionString)}");
        
        var databaseName = settings.DatabaseName;
        var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";
        return mongoDbConnectionString;
    }

    public static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(
            new MongoClient(GetMongoConnectionString(services)))
            .AddScoped(x=>x.GetService<IMongoClient>()?.StartSession());
    }
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddControllers();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        services.AddScoped<IInventoryService, InventoryService>();
        return services;
    }
}