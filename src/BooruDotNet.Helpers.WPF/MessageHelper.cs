using System.Windows;

namespace BooruDotNet.Helpers.WPF
{
    internal static class MessageHelper
    {
        internal static MessageBoxResult Warning(string message)
        {
            return ShowMessage(message, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private static MessageBoxResult ShowMessage(string message, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(message, image.ToString(), button, image);
        }
    }
}
