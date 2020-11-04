using System;

namespace BooruDotNet
{
    internal static class Error
    {
        internal static void If<T>(bool condition, params object?[]? ctorArgs) where T : Exception
        {
            if (condition)
            {
                throw (T)Activator.CreateInstance(typeof(T), ctorArgs)!;
            }
        }

        internal static void IfNot<T>(bool condition, params object?[]? ctorArgs) where T : Exception
        {
            If<T>(!condition, ctorArgs);
        }
    }
}
