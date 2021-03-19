using System;
using System.Diagnostics;
using System.IO;
using BooruDotNet.Resources;
using Validation;

namespace BooruDotNet.Extensions
{
    internal static class UriExtensions
    {
        [DebuggerStepThrough]
        internal static void RequireAbsolute(this Uri uri, string? argName = null)
        {
            Assumes.NotNull(uri);
            Requires.Argument(uri.IsAbsoluteUri, argName ?? nameof(uri), ErrorMessages.UriIsNotAbsolute);
        }

        internal static string GetFileName(this Uri uri)
        {
            Assumes.NotNull(uri);
            Assumes.True(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            return Path.GetFileName(uri.AbsolutePath);
        }

        internal static string GetExtension(this Uri uri)
        {
            Assumes.NotNull(uri);
            Assumes.True(uri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            return Path.GetExtension(uri.AbsolutePath);
        }
    }
}
