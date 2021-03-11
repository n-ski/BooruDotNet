using System;
using System.IO;
using BooruDotNet.Resources;
using Validation;

namespace BooruDotNet.Extensions
{
    internal static class UriExtensions
    {
        internal static string GetFileName(this Uri uri)
        {
            Assumes.NotNull(uri);
            Assumes.True(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            return Path.GetFileName(uri.AbsoluteUri);
        }

        internal static string GetExtension(this Uri uri)
        {
            Assumes.NotNull(uri);
            Assumes.True(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            return Path.GetExtension(uri.AbsoluteUri);
        }
    }
}
