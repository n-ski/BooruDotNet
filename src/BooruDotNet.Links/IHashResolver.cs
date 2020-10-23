using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;

namespace BooruDotNet.Links
{
    public interface IHashResolver : IResolver
    {
        Task<IPost?> ResolveFromHashLinkAsync(Uri uri, CancellationToken cancellationToken = default);
    }
}
