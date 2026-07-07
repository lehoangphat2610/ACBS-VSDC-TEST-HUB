using Acbs.Vsdc.TestHub.Domain;

namespace Acbs.Vsdc.TestHub.Services.Files;

public interface IFileIngestionService
{
    Task<long?> IngestAsync(
        string path,
        MessageDirection direction,
        GatewayFolderKind folderKind,
        CancellationToken cancellationToken,
        bool overwrite = false);
}
