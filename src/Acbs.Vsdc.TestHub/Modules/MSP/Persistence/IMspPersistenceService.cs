using Acbs.Vsdc.TestHub.Domain; using Acbs.Vsdc.TestHub.Services.Fin;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Persistence;
public interface IMspPersistenceService { Task PersistAsync(GatewayMessage message, ParsedFinMessage parsed, CancellationToken cancellationToken); }
