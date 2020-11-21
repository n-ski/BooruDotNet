using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;

namespace BooruDotNet.Search.Services
{
    public interface ISearchByUri
    {
        Task<IEnumerable<IResult>> SearchByAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
