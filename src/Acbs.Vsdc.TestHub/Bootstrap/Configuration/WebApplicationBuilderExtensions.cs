namespace Acbs.Vsdc.TestHub.Bootstrap;
public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddTestHubConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("Config/appsettings.hosting.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Config/appsettings.database.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Config/appsettings.gateway.json", optional: false, reloadOnChange: true)
            .AddJsonFile("Config/appsettings.msp.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "ACBS_VSDC_");
        return builder;
    }
}
