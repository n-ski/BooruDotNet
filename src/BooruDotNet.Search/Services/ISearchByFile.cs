using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;

namespace BooruDotNet.Search.Services
{
    public interface ISearchByFile
    {
        Task<IEnumerable<IResult>> SearchByAsync(FileStream fileStream, CancellationToken cancellationToken = default);
    }
}
