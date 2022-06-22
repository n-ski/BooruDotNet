using System;
using System.Runtime.InteropServices;

namespace BooruDotNet.Helpers;

internal static class NetHelper
{
    private static readonly Lazy<string> _userAgentLazy = new Lazy<string>(() =>
    {
        var framework = RuntimeInformation.FrameworkDescription;
        var words = framework.Split();

        return string.Concat(words[0..^1]) + "/" + words[^1];
    });

    internal static string UserAgentForRuntime => _userAgentLazy.Value;
}
