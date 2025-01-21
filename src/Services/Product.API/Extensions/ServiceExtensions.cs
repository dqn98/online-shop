using System.Text;
using AutoMapper;
using Contracts.Common.Interfaces;
using Contracts.Identity;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistences;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings))
            .Get<JwtSettings>();
        
        ArgumentNullException.ThrowIfNull(jwtSettings);
        
        services.AddSingleton(jwtSettings);
        
        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        // config swagger
        services.AddEndpointsApiExplorer();
        services.AddInfrastructureServices();
        services.ConfigureProductDbContext(configuration);
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddJwtAuthentication();
        
        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        if(settings is null || string.IsNullOrEmpty(settings.Key))
            throw new ArgumentNullException($"Jwt settings has not been configured. {nameof(JwtSettings)}");
        
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = false
        };
        
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParameters;
        });
        
        return services;
    }
    
    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString") ?? "";
        
        var builder = new MySqlConnectionStringBuilder(connectionString);

        services.AddDbContext<CatalogProductContext>(m => m.UseMySql(builder.ConnectionString,
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly("Product.API");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>));
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        
        services.AddTransient(typeof(IProductRepository), typeof(ProductRepository));
        services.AddTransient<ITokenService, TokenService>();
        
        return services;
    }
}