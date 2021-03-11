using System;
using System.Collections.Generic;
using Validation;

namespace BooruDotNet.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            Assumes.NotNull(source);
            Assumes.NotNull(action);

            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
