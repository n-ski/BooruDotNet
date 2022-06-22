using System;

namespace BooruDotNet.Helpers;

internal static class UriHelper
{
    internal static Uri CreateFormat(string format, params object?[] values)
    {
        return new Uri(string.Format(format, values));
    }
}
