using System.Windows;

namespace BooruDotNet.Downloader.Helpers
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
