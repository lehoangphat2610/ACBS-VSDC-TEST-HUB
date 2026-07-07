using System.Security.Cryptography;
using System.Text;

namespace Acbs.Vsdc.TestHub.Services.Files;

public static class FileHelpers
{
    public static async Task<bool> WaitUntilStableAsync(string path, int stableMilliseconds, CancellationToken cancellationToken)
    {
        long previousSize = -1;
        DateTime previousWrite = DateTime.MinValue;

        for (var attempt = 0; attempt < 10; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!File.Exists(path)) return false;

            try
            {
                var info = new FileInfo(path);
                if (info.Length == previousSize && info.LastWriteTimeUtc == previousWrite)
                {
                    await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return stream.Length >= 0;
                }

                previousSize = info.Length;
                previousWrite = info.LastWriteTimeUtc;
            }
            catch (IOException)
            {
                // File is still being copied by VSD Gateway.
            }

            await Task.Delay(stableMilliseconds, cancellationToken);
        }

        return false;
    }

    public static async Task<string> Sha256Async(string path, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    public static async Task WriteAtomicAsync(string targetPath, string content, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
        var tempPath = targetPath + ".tmp-" + Guid.NewGuid().ToString("N");
        await File.WriteAllTextAsync(tempPath, content, new UTF8Encoding(false), cancellationToken);
        File.Move(tempPath, targetPath, true);
    }

    public static string UniquePath(string folder, string fileName)
    {
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, fileName);
        if (!File.Exists(path)) return path;

        var stem = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        return Path.Combine(folder, $"{stem}-{DateTime.Now:HHmmssfff}{extension}");
    }
}
