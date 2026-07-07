namespace Acbs.Vsdc.TestHub.Areas.MSP.Models;
public sealed record MspOperationMenuItem(string Code,string Name,string Action,string MessageType);
public sealed class MspCreateMenuViewModel { public IReadOnlyList<MspOperationMenuItem> Items { get; init; }=[]; }
