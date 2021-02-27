using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BooruDotNet.Helpers.WPF
{
    internal static class ImageHelper
    {
        [return: NotNullIfNotNull("uri")]
        internal static ImageSource? CreateImageFromUri(Uri? uri)
        {
            if (uri is null)
            {
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage(uri);

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
