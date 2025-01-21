using Common.Logging;
using Product.API.Extensions;
using Product.API.Persistences;
using Scalar.AspNetCore;
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
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    app.UseInfrastructure();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
    
    app.MigrateDatabase<CatalogProductContext>((context, _) =>
    {
        CatalogProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    }).Run();
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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}