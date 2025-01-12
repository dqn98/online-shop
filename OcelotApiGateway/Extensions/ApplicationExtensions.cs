using Ocelot.Middleware;

namespace OcelotApiGateway.Extensions;

public static class ApplicationExtensions
{
    public static void UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
            
        app.UseCors("CorsPolicy");
        // app.UseMiddleware<ErrorWrappingMiddleware>();
        app.UseAuthorization();
        app.UseOcelot().Wait();
    }
}