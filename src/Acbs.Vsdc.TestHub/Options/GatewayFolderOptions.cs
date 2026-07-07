namespace Acbs.Vsdc.TestHub.Options;
public sealed class GatewayFolderOptions
{
    public string Send { get; set; } = "";
    public string Archive { get; set; } = "";
    public string Receive { get; set; } = "";
    public string Error { get; set; } = "";
    public string[] AllowedExtensions { get; set; } = [".fin", ".par", ".csv"];
    public int ScanIntervalMilliseconds { get; set; } = 1000;
    public int ReconciliationIntervalSeconds { get; set; } = 15;
    public int FileStableMilliseconds { get; set; } = 800;
    public bool IsAllowed(string path) => AllowedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
}
