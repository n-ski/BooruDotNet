using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooruDotNet.Tags;

namespace BooruDotNet.Namers
{
    public class DanbooruNamer : TagNamerBase
    {
        private const int _maxAllowedCharacterNames = 5;

        public DanbooruNamer(IBooruTagByName tagExtractor) : base(tagExtractor)
        {
        }

        public DanbooruNamer(Func<string, Task<ITag>> tagExtractorFunc) : base(tagExtractorFunc)
        {
        }

        // IMPORTANT: lists must be sorted by tag count descending.
        protected override string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags,
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
            if (copyrightTags.Count > 0)
            {
                nameBuilder.Append(copyrightTags[0].Name);

                if (copyrightTags.Count > 1)
                {
                    nameBuilder
                        .Append("_and_")
                        .Append(copyrightTags.Count - 1)
                        .Append("_more_");
                }
                else
                {
                    nameBuilder.Append('_');

                }
            }

            //// Step 3.
            // Remove words in brackets (at least this is what Danbooru does).
            // NOTE: Danbooru keeps artist's aliases, so we do this before
            // appending artist credits.
            nameBuilder = new StringBuilder(
                BracketRegex.Replace(nameBuilder.ToString(), ""));

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

        protected override string CreateName(int id, string hash)
        {
            return $"__{id}__{hash}";
        }
    }
}
