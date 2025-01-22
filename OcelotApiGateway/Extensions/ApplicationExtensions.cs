using Infrastructure.Middlewares;
using Ocelot.Middleware;

namespace OcelotApiGateway.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        });
            
        app.UseCors("CorsPolicy");
        app.UseMiddleware<ErrorWrappingMiddleware>();
        app.UseOcelot().Wait();
    }
}