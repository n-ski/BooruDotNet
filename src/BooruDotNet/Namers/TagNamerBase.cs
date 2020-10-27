using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
        protected virtual int MaxActiveTagRequestsCount => 5;

        private protected static Regex BracketRegex => _bracketRegexLazy.Value;

        public string Name(IPost post)
        {
            var artistTags = new List<ITag>();
            var copyrightTags = new List<ITag>();
            var characterTags = new List<ITag>();

            static void safeAdd(ICollection<ITag> list, ITag tag)
            {
                lock (list)
                {
                    list.Add(tag);
                }
            }

            using (var semaphore = new SemaphoreSlim(MaxActiveTagRequestsCount))
            {
                var tasks = post.Tags.Select(tagName =>
                {
                    semaphore.Wait();

                    return Task.Run(async () =>
                    {
                        try
                        {
                            ITag tag = await _tagExtractorFunc(tagName);

                            switch (tag.Kind)
                            {
                                case TagKind.Artist:
                                    safeAdd(artistTags, tag);
                                    break;
                                case TagKind.Copyright:
                                    safeAdd(copyrightTags, tag);
                                    break;
                                case TagKind.Character:
                                    safeAdd(characterTags, tag);
                                    break;
                            }
                        }
                        // Eat exception so the whole thing doesn't break.
                        catch
                        {
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                });

                Task.WhenAll(tasks).Wait();
            }

            static IEnumerable<ITag> orderByCountThenByName(IEnumerable<ITag> tags, bool descendNameOrder)
            {
                if (!tags.Any())
                {
                    return Array.Empty<ITag>();
                }

                static int getTagCount(ITag tag) => tag.Count;
                static string getTagName(ITag tag) => tag.Name;

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
                    artistTags,
                    post.Hash)
                : CreateName(post.ID, post.Hash);
        }

        protected abstract string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags,
            IReadOnlyList<ITag> artistTags, string hash);

        protected abstract string CreateName(int id, string hash);
    }
}
