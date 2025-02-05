using System.Security.Authentication;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Configurations;

namespace Infrastructure.ScheduledJobs;

public static class HangfireExtensions
{
    public static IServiceCollection AddCustomHangfireService(this IServiceCollection services)
    {
        var settings = services.GetOptions<HangFireSettings>(nameof(HangFireSettings));
        if (settings == null || settings.Storage is null || string.IsNullOrEmpty(settings.Storage.ConnectionString))
        {
             throw new ArgumentNullException(nameof(HangfireExtensions), "Hangfire settings are not configured");
        }
        
        services.ConfigureHangfireServices(settings);
        services.AddHangfireServer(serverOptions =>
        {
            serverOptions.ServerName = settings.ServerName;
        });

        return services;
    }
    
    private static IServiceCollection ConfigureHangfireServices(this IServiceCollection services, HangFireSettings hangFireSettings)
    {
        if (string.IsNullOrEmpty(hangFireSettings.Storage?.DbProvider))
        {
            throw new Exception("HangFireSettings Database provider is not configured");
        }

        switch (hangFireSettings.Storage.DbProvider.ToLower())
        {
            case "mongodb" :
                var mongoUrlBuilder = new MongoUrlBuilder(hangFireSettings.Storage.ConnectionString);
                var mongoClientSettings = MongoClientSettings.FromUrl(
                    new MongoUrl(hangFireSettings.Storage.ConnectionString));
                
                mongoClientSettings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
                var mongoClient = new MongoClient(mongoClientSettings);

                var mongoStorageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                    },
                    CheckConnection = true,
                    Prefix = "SchedulerQueue",
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                };
                
                
                
                services.AddHangfire((provider, config) =>
                {
                    config.UseSimpleAssemblyNameTypeSerializer()
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseRecommendedSerializerSettings()
                        // .UseConsole()               
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, mongoStorageOptions);

                    var jsonSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    };

                    config.UseSerializerSettings(jsonSettings);
                });
                services.AddHangfireConsoleExtensions();
                break;
            case "postgresql": 
                break;
            case "mssql":
                break;
            
            default:
                throw new Exception("HangFireSettings Database provider is not configured");
        }
        return services;
    }
}