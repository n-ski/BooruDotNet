using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BooruDownloader.Helpers
{
    internal static class ImageHelper
    {
        internal static ImageSource CreateImageFromUri(Uri uri)
        {
            var bitmapImage = new BitmapImage(uri);

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
