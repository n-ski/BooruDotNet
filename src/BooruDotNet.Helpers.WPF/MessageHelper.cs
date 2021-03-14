using System.Runtime.CompilerServices;
using System.Windows;

namespace BooruDotNet.Helpers
{
    internal static class MessageHelper
    {
        internal static void Information(string message, Window? owner = null)
        {
            ShowMessage(owner, message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal static void Warning(string message, Window? owner = null)
        {
            ShowMessage(owner, message, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static MessageBoxResult ShowMessage(Window? owner, string message,
            MessageBoxButton button, MessageBoxImage image, [CallerMemberName] string? caption = null)
        {
            return owner is null
                ? MessageBox.Show(message, caption!, button, image)
                : MessageBox.Show(owner, message, caption!, button, image);
        }
    }
}
