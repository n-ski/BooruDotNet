using System;
using System.Diagnostics;
using BooruDotNet.Extensions;

namespace BooruDownloader.Helpers
{
    internal static class UriHelper
    {
        internal static bool IsAnimatedMediaFile(Uri uri)
        {
            Debug.Assert(uri is object);

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
