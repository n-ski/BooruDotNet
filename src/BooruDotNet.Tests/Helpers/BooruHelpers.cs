using System;

namespace BooruDotNet.Tests.Helpers
{
    internal static class BooruHelpers
    {
        internal static T Create<T>(Type type)
        {
            T booru = (T)Activator.CreateInstance(type);
            return booru;
        }
    }
}
