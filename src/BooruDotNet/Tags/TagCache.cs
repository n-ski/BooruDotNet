using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Easy.Common;

namespace BooruDotNet.Tags
{
    public class TagCache
    {
        private readonly IBooruTagByName _tagExtractor;
        private readonly ConcurrentDictionary<string, Task<ITag>> _tags;

        public TagCache(IBooruTagByName tagExtractor, StringComparer? tagNameComparer = null)
        {
            _tagExtractor = Ensure.NotNull(tagExtractor, nameof(tagExtractor));
            _tags = new ConcurrentDictionary<string, Task<ITag>>(tagNameComparer ?? StringComparer.Ordinal);
        }

        public Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            return _tags.GetOrAdd(tagName, RunTask);
        }

        // Should be faster than specifying _tagExtractor.GetTagAsync as value factory
        // directly, since Task.Run returns immediately.
        private Task<ITag> RunTask(string tagName)
        {
            return Task.Run(() => _tagExtractor.GetTagAsync(tagName));
        }
    }
}
