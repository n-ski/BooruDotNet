using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BooruDotNet.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Debug.Assert(source is object);
            Debug.Assert(action is object);

            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
