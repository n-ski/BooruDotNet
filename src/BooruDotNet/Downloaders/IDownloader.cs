using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Downloaders
{
    public interface IDownloader
    {
        Task<FileInfo> DownloadAsync(object item, string targetDirectory, CancellationToken cancellationToken = default);
        IAsyncEnumerable<FileInfo> DownloadAsync(IEnumerable items, string targetDirectory,
            CancellationToken cancellationToken = default);
    }
}
