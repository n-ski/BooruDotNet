using System;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Caching
{
    public class TagCache : AsyncCacheBase<string, ITag>, IBooruTagByName
    {
        private readonly IBooruTagByName _tagExtractor;

        public TagCache(IBooruTagByName tagExtractor, StringComparer? tagNameComparer = null)
            : base(tagNameComparer ?? StringComparer.OrdinalIgnoreCase)
        {
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));
        }

        public Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            // Probably not a good idea to allow cancellation here, since it means we won't
            // be able to get this tag later, at least not until cache flushing is implemented.
            // See PostCache.cs.
            // TODO: Needs testing.
            return Cache.GetOrAdd(
                tagName,
                tag => Task.Run(
                    () => _tagExtractor.GetTagAsync(tag, cancellationToken),
                    cancellationToken));
        }
    }
}
