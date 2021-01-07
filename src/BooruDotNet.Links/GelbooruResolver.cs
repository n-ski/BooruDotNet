using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BooruDotNet.Boorus;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using Easy.Common;

namespace BooruDotNet.Links
{
    public class GelbooruResolver : ResolverBase, IIdResolver, IHashResolver
    {
        private static readonly Lazy<Regex> _gelbooruUriRegexLazy = new Lazy<Regex>(
            () => new Regex(@"^https?:\/\/gelbooru\.com/index\.php", RegexOptions.Compiled));

        public GelbooruResolver(Gelbooru gelbooru)
        {
            Gelbooru = Ensure.NotNull(gelbooru, nameof(gelbooru));
        }

        protected Gelbooru Gelbooru { get; }

        public async Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.That(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            if (IsGelbooruUri(uri))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);

                if (query["id"] is string value)
                {
                    int id = int.Parse(value);

                    return await Gelbooru.GetPostAsync(id, cancellationToken).ConfigureAwait(false);
                }
            }

            return null;
        }

        public async Task<IPost?> ResolveFromHashLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.That(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            if (IsGelbooruUri(uri))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);

                if (query["md5"] is string md5)
                {
                    return await Gelbooru.GetPostAsync(md5, cancellationToken).ConfigureAwait(false);
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
