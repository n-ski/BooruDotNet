using System;

namespace BooruDotNet.Search.WPF.Helpers
{
    internal static class UriHelper
    {
        internal static bool IsValid(Uri uri)
        {
            if (uri is null)
            {
                return false;
            }

            return uri.IsAbsoluteUri && uri.Host.Length > 0;
        }
    }
}
