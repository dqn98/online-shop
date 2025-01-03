﻿namespace Basket.API.Extensions;

public static class ApplicationExtension
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