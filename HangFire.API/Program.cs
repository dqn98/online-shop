using Common.Logging;
using HangFire.API.Extensions;
using Infrastructure.ScheduledJobs;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Configuration.AddAppConfigurations();
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.AddCustomHangfireService();
    
    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseRouting();
    app.UseAuthorization();
    app.UseHttpsRedirection();

    app.UseHangfireDashboard(builder.Configuration);

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
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}