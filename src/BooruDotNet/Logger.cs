using System.Diagnostics;

namespace BooruDotNet
{
    internal sealed class Logger
    {
        [Conditional("DEBUG")]
        internal static void Debug(string message, object? sender = null)
        {
            if (sender is null)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(message, sender.GetType().Name);
            }
        }
    }
}
