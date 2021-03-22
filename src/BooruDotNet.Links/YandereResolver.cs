using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Extensions;
using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Links
{
    public class YandereResolver : ResolverBase, IIdResolver
    {
        private static readonly Lazy<Regex> _idRegexLazy = new Lazy<Regex>(
            () => new Regex(@"^https:\/\/yande\.re\/post\/show\/(\d+)$", RegexOptions.Compiled));

        public YandereResolver(Yandere yandere)
        {
            Yandere = Requires.NotNull(yandere, nameof(yandere));
        }

        protected Yandere Yandere { get; }

        public async Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            Match match = _idRegexLazy.Value.Match(uri.AbsoluteUri);

            if (match.Success)
            {
                string value = match.Groups[1].Value;
                int id = int.Parse(value);

                return await Yandere.GetPostAsync(id, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
    }
}
