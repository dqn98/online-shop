namespace Inventory.API.Controllers;

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

    }
}