using System;
using System.Diagnostics.CodeAnalysis;

namespace BooruDotNet;

internal static class Error
{
    internal static void If<T>([DoesNotReturnIf(true)] bool condition, params object?[]? ctorArgs) where T : Exception
    {
        if (condition)
        {
            throw (T)Activator.CreateInstance(typeof(T), ctorArgs)!;
        }
    }

    internal static void IfNot<T>([DoesNotReturnIf(false)] bool condition, params object?[]? ctorArgs) where T : Exception
    {
        If<T>(!condition, ctorArgs);
    }
}
