using System;
using BooruDotNet.Extensions;
using Validation;

namespace BooruDownloader.Helpers
{
    internal static class UriHelper
    {
        internal static bool IsAnimatedMediaFile(Uri uri)
        {
            Assumes.NotNull(uri);

            switch (uri.GetExtension().ToLowerInvariant())
            {
                case ".gif":
                case ".mp4":
                case ".webm":
                    return true;
            }

            return false;
        }
    }
}
