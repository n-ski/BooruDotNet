﻿using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Boorus;
using BooruDotNet.Posts;
using BooruDotNet.Resources;
using Easy.Common;

namespace BooruDotNet.Links
{
    public class DanbooruResolver : ResolverBase, IIdResolver
    {
        private static readonly Lazy<Regex> _idRegexLazy = new Lazy<Regex>(
            () => new Regex(@"^https:\/\/danbooru\.donmai\.us\/posts\/(\d+)$", RegexOptions.Compiled));

        public DanbooruResolver(Danbooru danbooru)
        {
            Danbooru = Ensure.NotNull(danbooru, nameof(danbooru));
        }

        protected Danbooru Danbooru { get; }

        public async Task<IPost?> ResolveFromIdLinkAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.That(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            Match match = _idRegexLazy.Value.Match(uri.AbsoluteUri);

            if (match.Success)
            {
                string value = match.Groups[1].Value;
                int id = int.Parse(value);

                return await Danbooru.GetPostAsync(id, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
    }
}