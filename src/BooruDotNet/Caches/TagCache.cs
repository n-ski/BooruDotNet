using System;
using System.Threading.Tasks;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Caches
{
    public class TagCache : AsyncCacheBase<string, ITag>, IBooruTagByName
    {
        private readonly IBooruTagByName _tagExtractor;

        public TagCache(IBooruTagByName tagExtractor, StringComparer? tagNameComparer = null)
            : base(tagNameComparer ?? StringComparer.OrdinalIgnoreCase)
        {
            _tagExtractor = Ensure.NotNull(tagExtractor, nameof(tagExtractor));
        }

        public Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            return Cache.GetOrAdd(tagName, RunTask);
        }

        // Should be faster than specifying _tagExtractor.GetTagAsync as value factory
        // directly, since Task.Run returns immediately.
        private Task<ITag> RunTask(string tagName)
        {
            return Task.Run(() => _tagExtractor.GetTagAsync(tagName));
        }
    }
}
