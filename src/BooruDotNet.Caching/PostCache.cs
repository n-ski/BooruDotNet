using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;
using Easy.Common;

namespace BooruDotNet.Caching
{
    public class PostCache : AsyncCacheBase<int, IPost>, IBooruPostById
    {
        private readonly IBooruPostById _postExtractor;

        public PostCache(IBooruPostById postExtractor)
        {
            _postExtractor = Ensure.NotNull(postExtractor, nameof(postExtractor));
        }

        public Task<IPost> GetPostAsync(int postId, CancellationToken cancellationToken = default)
        {
            // Probably not a good idea to allow cancellation here, since it means we won't
            // be able to get this post later, at least not until cache flushing is implemented.
            // See TagCache.cs.
            // TODO: Needs testing.
            return Cache.GetOrAdd(
                postId,
                id => Task.Run(
                    () => _postExtractor.GetPostAsync(id, cancellationToken),
                    cancellationToken));
        }
    }
}
