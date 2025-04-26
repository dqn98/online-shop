using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
            services.AddTransient<ICheckoutSagaService, CheckoutSagaService>()
                .AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, OrderManager.OrderManager>();   

    public static IServiceCollection ConfigureHttpRepository(this IServiceCollection services) =>
        services.AddScoped<IOrderHttpRepository, OrdeRHttpRepository>()
            .AddScoped<IBasketHttpRepository, BasketHttpRepository>()
            .AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();

    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.ConfigureOrderHttpClient();
        services.ConfigureBasketHttpClient();
        services.ConfigureInventoryHttpClient();
        return services;
    }
    
    private static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrdeRHttpRepository>("OrdersAPI", (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5005/api/v1/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrdersAPI"));
    }
    
    private static void ConfigureBasketHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrdeRHttpRepository>("BasketsAPI", (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5004/api/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketsAPI"));
    }
    
    private static void ConfigureInventoryHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrdeRHttpRepository>("InventoryAPI", (serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5006/api/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }
}