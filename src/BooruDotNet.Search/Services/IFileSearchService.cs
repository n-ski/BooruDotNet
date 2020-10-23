using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;

namespace BooruDotNet.Search.Services
{
    public interface IFileSearchService
    {
        long FileSizeLimit { get; }

        Task<IEnumerable<IResult>> SearchAsync(FileStream fileStream, CancellationToken cancellationToken = default);
    }
}
