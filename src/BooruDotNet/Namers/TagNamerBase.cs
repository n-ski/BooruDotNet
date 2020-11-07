using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Namers
{
    public abstract class TagNamerBase : IPostNamer
    {
        private readonly Func<string, Task<ITag>> _tagExtractorFunc;
        private static readonly Lazy<Regex> _bracketRegexLazy = new Lazy<Regex>(
            () => new Regex(@"_\(.+?\)", RegexOptions.Compiled));

        protected TagNamerBase(IBooruTagByName tagExtractor)
            : this(Ensure.NotNull(tagExtractor, nameof(tagExtractor)).GetTagAsync)
        {
        }

        protected TagNamerBase(Func<string, Task<ITag>> tagExtractorFunc)
        {
            _tagExtractorFunc = Ensure.NotNull(tagExtractorFunc, nameof(tagExtractorFunc));
        }

        // Allow this many requests at once when retrieving tags.
        protected virtual int MaxActiveTagRequestsCount => 6;

        private protected static Regex BracketRegex => _bracketRegexLazy.Value;

        public string Name(IPost post)
        {
            var artistTags = new ConcurrentQueue<ITag>();
            var copyrightTags = new ConcurrentQueue<ITag>();
            var characterTags = new ConcurrentQueue<ITag>();

            Parallel.ForEach(
                Partitioner.Create(post.Tags, EnumerablePartitionerOptions.NoBuffering),
                new ParallelOptions { MaxDegreeOfParallelism = MaxActiveTagRequestsCount },
                tagName =>
                {
                    try
                    {
                        ITag tag = _tagExtractorFunc(tagName).Result;

                        switch (tag.Kind)
                        {
                            case TagKind.Artist:
                                artistTags.Enqueue(tag);
                                break;
                            case TagKind.Copyright:
                                copyrightTags.Enqueue(tag);
                                break;
                            case TagKind.Character:
                                characterTags.Enqueue(tag);
                                break;
                        }
                    }
                    catch
                    {
                    }
                });

            static int getTagCount(ITag tag) => tag.Count;
            static string getTagName(ITag tag) => tag.Name;

            static IEnumerable<ITag> orderByCountThenByName(IEnumerable<ITag> tags, bool descendNameOrder)
            {
                if (!tags.Any())
                {
                    return Array.Empty<ITag>();
                }

                var byCountDescend = tags.OrderByDescending(getTagCount);
                return descendNameOrder
                    ? byCountDescend.ThenByDescending(getTagName)
                    : byCountDescend.ThenBy(getTagName);
            }

            return characterTags.Count > 0 || copyrightTags.Count > 0 || artistTags.Count > 0
                ? CreateName(
                    orderByCountThenByName(characterTags, true).ToArray(),
                    // TODO: figure out how copyright tags are sorted.
                    orderByCountThenByName(copyrightTags, true).ToArray(),
                    artistTags.OrderBy(getTagName).ToArray(),
                    post.Hash)
                : CreateName(post.ID ?? 0, post.Hash);
        }

        protected abstract string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags,
            IReadOnlyList<ITag> artistTags, string hash);

        protected abstract string CreateName(int id, string hash);
    }
}
