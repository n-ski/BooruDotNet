using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Downloaders
{
    public interface IDownloader
    {
        Task DownloadAsync(object item, string targetDirectory, CancellationToken cancellationToken = default);
        Task DownloadAsync(IEnumerable items, string targetDirectory, CancellationToken cancellationToken = default);
    }
}
