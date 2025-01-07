using Inventory.API.Controllers;
using Inventory.API.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");
try
{
    builder.Services.AddOpenApi();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructureServices();
    builder.Services.ConfigureMongoDbClient();
    var app = builder.Build();

    app.UseInfrastructure();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MigrateDatabase();
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
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}