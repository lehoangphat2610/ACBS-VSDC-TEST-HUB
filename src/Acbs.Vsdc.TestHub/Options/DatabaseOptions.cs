namespace Acbs.Vsdc.TestHub.Options;
public sealed class DatabaseOptions
{
    public string Provider { get; set; } = "SqlServer";
    public bool EnsureCreated { get; set; } = true;
    public bool EnableSensitiveDataLogging { get; set; }
    public int CommandTimeoutSeconds { get; set; } = 60;
}
