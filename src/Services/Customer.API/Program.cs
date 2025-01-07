using Common.Logging;
using Customer.API.Extensions;
using Customer.API.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Services.AddOpenApi();
    builder.Configuration.AddAppConfigurations();

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    app.UseInfrastructure();
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapGet("/api/customers", 
        async (ICustomerServices customerServices) => await customerServices.GetCustomers());
    app.MapGet("/api/customer/{username}", 
        async (string username, ICustomerServices customerServices) =>
        {
            var customer = await customerServices.GetCustomerByUserNameAsync(username);
            return Results.Ok(customer);
        });
    
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
    Log.Information("Shut down Customer API complete");
    Log.CloseAndFlush();
}