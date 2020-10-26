using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Easy.Common;

namespace BooruDotNet.Namers
{
    public class DanbooruNamer : IPostNamer
    {
        private const int _maxAllowedCharacterNames = 5;
        // Allow this many requests at once when retrieving tags.
        private const int _maxSemaphoreCount = 5;
        private static readonly Lazy<Regex> _bracketRegexLazy = new Lazy<Regex>(
            () => new Regex(@"_\(.+?\)", RegexOptions.Compiled));
        private readonly Func<string, Task<ITag>> _tagExtractorFunc;

        public DanbooruNamer(IBooruTagByName tagExtractor)
            : this(Ensure.NotNull(tagExtractor, nameof(tagExtractor)).GetTagAsync)
        {
        }

        public DanbooruNamer(Func<string, Task<ITag>> tagExtractorFunc)
        {
            _tagExtractorFunc = Ensure.NotNull(tagExtractorFunc, nameof(tagExtractorFunc));
        }

        public string Name(IPost post)
        {
            using var semaphore = new SemaphoreSlim(_maxSemaphoreCount);

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
                    orderByCountThenByName(artistTags, false).ToArray(),
                    post.Hash)
                : $"__{post.ID}__{post.Hash}";
        }

        // IMPORTANT: lists must be sorted by tag count descending.
        private string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags,
            IReadOnlyList<ITag> artistTags, string hash)
        {
            static StringBuilder joinTagsWithPrefix(
                StringBuilder sb, string separator, string prefix, IReadOnlyList<ITag> tags)
            {
                int lessByOneCount = tags.Count - 1;

                for (int i = 0; i < lessByOneCount; i++)
                {
                    sb.Append(tags[i].Name).Append(separator);
                }

                return sb
                    .Append(prefix)
                    .Append(separator)
                    .Append(tags[lessByOneCount].Name);
            }

            StringBuilder nameBuilder = new StringBuilder();

            //// Step 1.
            // Append characters.
            if (characterTags.Count > _maxAllowedCharacterNames)
            {
                foreach (ITag tag in characterTags.Take(_maxAllowedCharacterNames))
                {
                    nameBuilder.Append(tag.Name).Append('_');
                }

                nameBuilder
                    .Append("and_")
                    .Append(characterTags.Count - _maxAllowedCharacterNames)
                    .Append("_more_");
            }
            else if (characterTags.Count > 1)
            {
                joinTagsWithPrefix(nameBuilder, "_", "and", characterTags).Append('_');
            }
            else if (characterTags.Count == 1)
            {
                nameBuilder.Append(characterTags[0].Name).Append('_');
            }

            //// Step 2.
            // Append copyrights.
            if (copyrightTags.Count > 1)
            {
                nameBuilder
                    // Get the most popular tag.
                    .Append(copyrightTags[0].Name)
                    .Append("_and_")
                    .Append(copyrightTags.Count - 1)
                    .Append("_more_");
            }
            else if (copyrightTags.Count == 1)
            {
                nameBuilder.Append(copyrightTags[0].Name).Append('_');
            }

            //// Step 3.
            // Remove words in brackets (at least this is what Danbooru does).
            // NOTE: Danbooru keeps artist's aliases, so we do this before
            // appending artist credits.
            nameBuilder = new StringBuilder(
                _bracketRegexLazy.Value.Replace(nameBuilder.ToString(), ""));

            //// Step 4.
            // Append artists.
            if (artistTags.Count >= 1)
            {
                nameBuilder.Append("drawn_by_");

                if (artistTags.Count > 1)
                {
                    joinTagsWithPrefix(nameBuilder, "_", "and", artistTags).Append('_');
                }
                else
                {
                    nameBuilder.Append(artistTags[0].Name).Append('_');
                }
            }

            //// Step 5.
            // Remove/replace garbage chars we don't care about.
            var charList = new List<char>(nameBuilder.ToString().ToCharArray());
            // Don't include the first and the last characters.
            for (int i = charList.Count - 2; i > 0; i--)
            {
                char c = charList[i];
                if (!char.IsLetterOrDigit(c))
                {
                    // Remove this character if there is
                    // undescore to the either left or right.
                    if (charList[i - 1] == '_' || charList[i + 1] == '_')
                    {
                        charList.RemoveAt(i);
                    }
                    // Otherwise replace this char with underscore.
                    else
                    {
                        charList[i] = '_';
                    }
                }
            }

            //// Step 6.
            // Combine everything.
            return string.Concat("__", new string(charList.ToArray()), "_", hash);
        }
    }
}
