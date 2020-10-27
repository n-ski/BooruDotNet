using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BooruDotNet.Tags;

namespace BooruDotNet.Namers
{
    public class DanbooruFancyNamer : TagNamerBase
    {
        private const int _maxAllowedCharacterNames = 5;
        private static readonly Lazy<Regex> _underscoreRegexLazy = new Lazy<Regex>(
            () => new Regex(@"(?<=\w)_(?=\w)"));

        public DanbooruFancyNamer(IBooruTagByName tagExtractor) : base(tagExtractor)
        {
        }

        public DanbooruFancyNamer(Func<string, Task<ITag>> tagExtractorFunc) : base(tagExtractorFunc)
        {
        }

        protected override string CreateName(IReadOnlyList<ITag> characterTags, IReadOnlyList<ITag> copyrightTags, IReadOnlyList<ITag> artistTags, string hash)
        {
            StringBuilder nameBuilder = new StringBuilder();

            //// Step 1.
            // Append characters.
            if (characterTags.Count > _maxAllowedCharacterNames)
            {
                foreach (ITag tag in characterTags.Take(_maxAllowedCharacterNames))
                {
                    nameBuilder.Append(tag.Name).Append(", ");
                }

                nameBuilder
                    .Append("and ")
                    .Append(characterTags.Count - _maxAllowedCharacterNames)
                    .Append(" more ");
            }
            else if (characterTags.Count > 2)
            {
                int lengthExceptLast = characterTags.Count - 1;

                for (int i = 0; i < lengthExceptLast; i++)
                {
                    nameBuilder.Append(characterTags[i].Name).Append(", ");
                }

                nameBuilder
                    .Append("and ")
                    .Append(characterTags[lengthExceptLast].Name)
                    .Append(' ');
            }
            else if (characterTags.Count == 2)
            {
                nameBuilder
                    .Append(characterTags[0].Name)
                    .Append(" and ")
                    .Append(characterTags[1].Name)
                    .Append(' ');
            }
            else if (characterTags.Count == 1)
            {
                nameBuilder.Append(characterTags[0].Name).Append(' ');
            }
            else
            {
                nameBuilder.Append('_');
            }

            //// Step 2.
            // Remove words enclosed in brackets.
            nameBuilder = new StringBuilder(
                BracketRegex.Replace(nameBuilder.ToString(), ""));

            //// Step 3.
            // Append copyrights.
            if (copyrightTags.Count > 0)
            {
                bool hasCharacters = characterTags.Count > 0;

                if (hasCharacters)
                {
                    nameBuilder.Append('(');
                }

                nameBuilder.Append(BracketRegex.Replace(copyrightTags[0].Name, ""));

                if (copyrightTags.Count > 1)
                {
                    nameBuilder
                        .Append(" and ")
                        .Append(copyrightTags.Count - 1)
                        .Append(" more");
                }

                if (hasCharacters)
                {
                    nameBuilder.Append(')');
                }
            }

            //// Step 4.
            // Replace all underscores between words with spaces.
            nameBuilder = new StringBuilder(
                _underscoreRegexLazy.Value.Replace(nameBuilder.ToString(), " "));

            //// Step 5.
            // Append artists.
            if (artistTags.Count > 0)
            {
                nameBuilder.Append(" drawn by ");

                if (artistTags.Count > 2)
                {
                    // TODO: how many artists are allowed?
                    throw new NotImplementedException();
                }
                else
                {
                    nameBuilder.Append(artistTags[0].Name);

                    if (artistTags.Count == 2)
                    {
                        nameBuilder
                            .Append(" and ")
                            .Append(artistTags[1].Name);
                    }
                }
            }
            else
            {
                nameBuilder.Append(' ');
            }

            //// Step 6.
            // Finishing touches.
            return nameBuilder.Append(" - ").Append(hash).ToString();
        }

        protected override string CreateName(int id, string hash)
        {
            return $"#{id} - {hash}";
        }
    }
}
