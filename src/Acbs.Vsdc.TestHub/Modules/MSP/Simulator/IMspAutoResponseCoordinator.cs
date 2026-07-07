namespace Acbs.Vsdc.TestHub.Modules.Msp.Simulator;
public interface IMspAutoResponseCoordinator { Task<IReadOnlyList<(string FileName,string Content)>> CreateResponsesAsync(string raw,string result,string? rejectReason,CancellationToken cancellationToken); }
