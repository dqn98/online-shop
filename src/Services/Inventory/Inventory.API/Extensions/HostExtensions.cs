using Inventory.API.Persistences;
using MongoDB.Driver;

namespace Inventory.API.Extensions;

public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var settings = services.GetService<DatabaseSettings>();
        ArgumentNullException.ThrowIfNull(settings);
        
        if(string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException(
                $"Database connection string is empty. {nameof(DatabaseSettings.ConnectionString)}");
        
        var mongoClient = services.GetService<IMongoClient>();
        ArgumentNullException.ThrowIfNull(mongoClient);
        
        new InventoryDbSeed().SeedDataAsync(mongoClient, settings).Wait();
        return host;
    }
}