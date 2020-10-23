using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BooruDotNet.Extensions;
using BooruDotNet.Links;
using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Boorus
{
    public class GelbooruResolver : ResolverBase, IIdResolver, IHashResolver
    {
        private static readonly Lazy<Regex> _gelbooruUriRegexLazy = new Lazy<Regex>(
            () => new Regex(@"^https?:\/\/gelbooru\.com/index\.php", RegexOptions.Compiled));

        public GelbooruResolver(Gelbooru gelbooru)
        {
            Gelbooru = Requires.NotNull(gelbooru, nameof(gelbooru));
        }

        protected Gelbooru Gelbooru { get; }

        public async Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            if (IsGelbooruUri(uri))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);

                if (query["id"] is string value)
                {
                    int id = int.Parse(value);

                    return await Gelbooru.GetPostAsync(id, cancellationToken).CAF();
                }
            }

            return null;
        }

        public async Task<IPost?> ResolveFromHashLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(uri, nameof(uri));
            uri.RequireAbsolute();

            if (IsGelbooruUri(uri))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);

                if (query["md5"] is string md5)
                {
                    return await Gelbooru.GetPostAsync(md5, cancellationToken).CAF();
                }
            }

            return null;
        }

        private static bool IsGelbooruUri(Uri uri)
        {
            return _gelbooruUriRegexLazy.Value.IsMatch(uri.AbsoluteUri);
        }
    }
}
