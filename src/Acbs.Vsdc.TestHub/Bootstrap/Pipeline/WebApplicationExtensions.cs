namespace Acbs.Vsdc.TestHub.Bootstrap;
public static class WebApplicationExtensions
{
    public static WebApplication UseTestHubPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "msp-root",
            pattern: "MSP",
            defaults: new { area = "MSP", controller = "MspTest", action = "Index" });
        app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapControllers();
        return app;
    }
}
