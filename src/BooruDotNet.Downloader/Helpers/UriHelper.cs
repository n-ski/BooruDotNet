using System;
using BooruDotNet.Extensions;
using Easy.Common;

namespace BooruDotNet.Downloader.Helpers
{
    internal static class UriHelper
    {
        internal static bool IsAnimatedMediaFile(Uri uri)
        {
            Ensure.NotNull(uri, nameof(uri));

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
