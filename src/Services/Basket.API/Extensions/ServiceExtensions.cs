using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory.Grpc.Protos;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
            .Get<EventBusSettings>();
        
        var cacheSettings = configuration.GetSection(nameof(CacheSettings))
            .Get<CacheSettings>();
                
        var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
            .Get<GrpcSettings>();
        
        ArgumentNullException.ThrowIfNull(eventBusSettings);
        ArgumentNullException.ThrowIfNull(cacheSettings);
        ArgumentNullException.ThrowIfNull(grpcSettings);
        
        services.AddSingleton(eventBusSettings);
        services.AddSingleton(cacheSettings);
        services.AddSingleton(grpcSettings);
        
        return services;
    }
    
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
        => services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();

    public static IServiceCollection ConfigureRedis(
        this IServiceCollection services, 
        IConfiguration configuration
    )
    {
        var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        if(string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentNullException(
                $"Redis connection string is empty. {nameof(CacheSettings.ConnectionString)}");
        
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
        });
        services.AddDistributedMemoryCache();
        return services;
    }
    
    public static IServiceCollection ConfigureGrpcServices(this IServiceCollection services)
    {
        var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
        if(string.IsNullOrEmpty(settings.StockUrl))
            throw new ArgumentNullException(
                $"Grpc settings is not configured. {nameof(settings.StockUrl)}");
        
        services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(x 
            => x.Address = new Uri(settings.StockUrl));
        services.AddScoped<StockItemGrpcService>();

        return services;
    }

    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
        if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
            throw new ArgumentNullException(
                $"EventBusSettings is not configured : {nameof(settings.HostAddress)}");
        
        var mqConnection = new Uri(settings.HostAddress);
        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(mqConnection);
            });
            config.AddRequestClient<IBasketCheckoutEvent>();
        });
    }
}