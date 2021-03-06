using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Extensions;
using BooruDotNet.Links;
using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Konachan
{
    public class KonachanResolver : ResolverBase, IIdResolver
    {
        private static readonly Lazy<Regex> _idRegexLazy = new Lazy<Regex>(
            () => new Regex(@"^https:\/\/konachan\.com\/post\/show\/(\d+)", RegexOptions.Compiled));

        public KonachanResolver(Konachan konachan)
        {
            Konachan = Requires.NotNull(konachan, nameof(konachan));
        }

        protected Konachan Konachan { get; }

        public async Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            Match match = _idRegexLazy.Value.Match(uri.AbsoluteUri);

            if (match.Success)
            {
                string value = match.Groups[1].Value;
                int id = int.Parse(value);

                return await Konachan.GetPostAsync(id, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
    }
}
