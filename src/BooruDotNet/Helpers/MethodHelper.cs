using System.Diagnostics;

namespace BooruDotNet.Helpers;

internal static class MethodHelper
{
    [DebuggerStepThrough]
    internal static void DoNothing()
    {
    }

    [DebuggerStepThrough]
    internal static void DoNothing<T>(T _)
    {
    }
}
