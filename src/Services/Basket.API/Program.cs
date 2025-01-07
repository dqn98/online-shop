using Basket.API;
using Basket.API.Extensions;
using Common.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.AddOpenApi();
    builder.Configuration.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddAutoMapper(
        cfg => cfg.AddProfile(new MappingProfile()));
    
    builder.Services.ConfigureServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.ConfigureGrpcServices();
    builder.Services.Configure<RouteOptions>(
        options => options.LowercaseUrls = true);
    
    //Configure mass transit
    builder.Services.ConfigureMassTransit(builder.Configuration);
    
    builder.Services.AddControllers();
    var app = builder.Build();
    app.UseInfrastructure();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // app.UseHttpsRedirection();
    
    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}