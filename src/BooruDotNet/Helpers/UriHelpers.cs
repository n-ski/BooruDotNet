using System;

namespace BooruDotNet.Helpers
{
    internal static class UriHelpers
    {
        internal static Uri CreateFormat(string format, params object[] values)
        {
            return new Uri(string.Format(format, values));
        }
    }
}
