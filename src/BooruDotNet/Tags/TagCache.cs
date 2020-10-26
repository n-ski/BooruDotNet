using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easy.Common;

namespace BooruDotNet.Tags
{
    public class TagCache
    {
        private readonly IBooruTagByName _tagExtractor;
        private readonly Dictionary<string, Task<ITag>> _tags;

        public TagCache(IBooruTagByName tagExtractor, StringComparer? tagNameComparer = null)
        {
            _tagExtractor = Ensure.NotNull(tagExtractor, nameof(tagExtractor));
            _tags = new Dictionary<string, Task<ITag>>(tagNameComparer ?? StringComparer.Ordinal);
        }

        public Task<ITag> GetTagAsync(string tagName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(tagName);

            lock (_tags)
            {
                if (!_tags.TryGetValue(tagName, out var tagTask))
                {
                    tagTask = Task.Run(() => _tagExtractor.GetTagAsync(tagName));
                    _tags[tagName] = tagTask;
                }

                return tagTask;
            }
        }
    }
}
