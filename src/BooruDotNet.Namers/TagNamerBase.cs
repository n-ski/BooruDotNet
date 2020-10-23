using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BooruDotNet.Extensions;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using Validation;

namespace BooruDotNet.Namers
{
    public abstract class TagNamerBase : IPostNamer
    {
        private readonly Func<string, Task<ITag>> _tagExtractorFunc;
        private static readonly Lazy<Regex> _bracketRegexLazy = new Lazy<Regex>(
            () => new Regex(@"_\(.+?\)", RegexOptions.Compiled));
        private static readonly DataflowLinkOptions _linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

        protected TagNamerBase(IBooruTagByName tagExtractor)
            : this(tag => Requires.NotNull(tagExtractor, nameof(tagExtractor)).GetTagAsync(tag))
        {
        }

        protected TagNamerBase(Func<string, Task<ITag>> tagExtractorFunc)
        {
            _tagExtractorFunc = Requires.NotNull(tagExtractorFunc, nameof(tagExtractorFunc));
        }

        // Allow this many requests at once when retrieving tags.
        protected virtual int MaxActiveTagRequestsCount => 6;

        private protected static Regex BracketRegex => _bracketRegexLazy.Value;

        public string Name(IPost post)
        {
            // Keep tags separated. This is faster than enumerating
            // over a large collection with all the tags using LINQ.
            var artistTags = new List<ITag>();
            var copyrightTags = new List<ITag>();
            var characterTags = new List<ITag>();

            // Put tags into their own lists.
            var addTagBlock = new ActionBlock<ITag?>(tag =>
            {
                switch (tag?.Kind)
                {
                    case TagKind.Artist:
                        artistTags.Add(tag);
                        break;
                    case TagKind.Copyright:
                        copyrightTags.Add(tag);
                        break;
                    case TagKind.Character:
                        characterTags.Add(tag);
                        break;
                }
            });

            if (post is IPostExtendedTags postExtendedTags)
            {
                postExtendedTags.ExtendedTags.ForEach(tag => addTagBlock.Post(tag));

                addTagBlock.Complete();
            }
            else
            {
                // Get tags using provided function in parallel.
                var getTagBlock = new TransformBlock<string, ITag?>(
                    async tagName =>
                    {
                        try
                        {
                            return await _tagExtractorFunc(tagName).CAF();
                        }
                        catch (InvalidTagNameException)
                        {
                            Logger.Debug($"Got invalid tag '{tagName}'.", this);
                            return null;
                        }
                    },
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = MaxActiveTagRequestsCount });

                // IMPORTANT: getTagBlock must propagate its completion if we want to wait for
                // addTagBlock's completion. This is the correct usage as per MS docs.
                getTagBlock.LinkTo(addTagBlock, _linkOptions);

                if (post is IPostExtraTags extraTags)
                {
                    void postTag(string tag) => getTagBlock.Post(tag);

                    extraTags.CharacterTags.ForEach(postTag);
                    extraTags.CopyrightTags.ForEach(postTag);
                    extraTags.ArtistTags.ForEach(postTag);
                }
                else
                {
                    foreach (string tag in post.Tags)
                    {
                        getTagBlock.Post(tag);
                    }
                }

                getTagBlock.Complete();
            }

            // Wait until all the tags are processed.
            addTagBlock.Completion.Wait();

            // Keeping these as static methods should make calls to these faster.
            static int getTagCount(ITag tag) => tag.Count;
            static string getTagName(ITag tag) => tag.Name;

            static IEnumerable<ITag> orderByCountThenByName(IEnumerable<ITag> tags)
            {
                return tags
                    .OrderByDescending(getTagCount)
                    .ThenByDescending(getTagName);
            }

            return characterTags.Count > 0 || copyrightTags.Count > 0 || artistTags.Count > 0
                ? CreateName(
                    orderByCountThenByName(characterTags).ToArray(),
                    // TODO: figure out how copyright tags are sorted.
                    orderByCountThenByName(copyrightTags).ToArray(),
                    artistTags.OrderBy(getTagName).ToArray(),
                    post.Hash)
                : CreateName(post.ID ?? 0, post.Hash);
        }

        protected abstract string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags,
            IReadOnlyList<ITag> artistTags, string hash);

        protected abstract string CreateName(int id, string hash);
    }
}
