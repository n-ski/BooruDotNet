using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Links
{
    public static class LinkResolver
    {
        private static readonly ICollection<IIdResolver> _idResolvers = new List<IIdResolver>();
        private static readonly ICollection<IHashResolver> _hashResolvers = new List<IHashResolver>();

        public static void RegisterResolver(IResolver resolver)
        {
            Requires.NotNull(resolver, nameof(resolver));

            if (resolver is IIdResolver idResolver)
            {
                _idResolvers.Add(idResolver);
            }

            if (resolver is IHashResolver hashResolver)
            {
                _hashResolvers.Add(hashResolver);
            }
        }

        public static async Task<IPost?> ResolveAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            // Validation is done by the resolvers.
            // Keep trying each resolver until one of them returns successfully.
            IPost? post;

            foreach (var idResolver in _idResolvers)
            {
                post = await idResolver.ResolveFromIdLinkAsync(uri, cancellationToken).ConfigureAwait(false);

                if (post is object)
                {
                    return post;
                }
            }

            foreach (var hashResolver in _hashResolvers)
            {
                post = await hashResolver.ResolveFromHashLinkAsync(uri, cancellationToken).ConfigureAwait(false);

                if (post is object)
                {
                    return post;
                }
            }

            return null;
        }
    }
}
