using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;

namespace BooruDotNet.Links
{
    public interface IIdResolver : IResolver
    {
        Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
