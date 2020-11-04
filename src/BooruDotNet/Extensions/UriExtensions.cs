using System;
using System.IO;
using Easy.Common;

namespace BooruDotNet.Extensions
{
    internal static class UriExtensions
    {
        internal static string GetFileName(this Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));
            Ensure.That(uri.IsAbsoluteUri, "URI is not absolute.");

            return Path.GetFileName(uri.AbsoluteUri);
        }
    }
}
