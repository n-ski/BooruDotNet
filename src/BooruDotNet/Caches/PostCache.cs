using System.Threading.Tasks;
using BooruDotNet.Posts;
using Easy.Common;

namespace BooruDotNet.Caches
{
    public class PostCache : AsyncCacheBase<int, IPost>, IBooruPostById
    {
        private readonly IBooruPostById _postExtractor;

        public PostCache(IBooruPostById postExtractor)
        {
            _postExtractor = Ensure.NotNull(postExtractor, nameof(postExtractor));
        }

        public Task<IPost> GetPostAsync(int postId)
        {
            return Cache.GetOrAdd(postId, RunTask);
        }

        // Should be faster than specifying _postExtractor.GetPostAsync as value factory
        // directly, since Task.Run returns immediately.
        private Task<IPost> RunTask(int postId)
        {
            return Task.Run(() => _postExtractor.GetPostAsync(postId));
        }
    }
}
