using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BooruDotNet.Helpers
{
    internal static class ImageHelper
    {
        [return: NotNullIfNotNull("uri")]
        internal static BitmapSource? CreateImageFromUri(Uri? uri)
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

        internal static BitmapSource CreateImageFromStream(Stream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        internal static BitmapSource ScaleImage(BitmapSource source, double scale)
        {
            TransformedBitmap transformed = new TransformedBitmap();
            transformed.BeginInit();
            transformed.Source = source;
            transformed.Transform = new ScaleTransform(scale, scale);
            transformed.EndInit();

            if (transformed.CanFreeze)
            {
                transformed.Freeze();
            }

            return transformed;
        }

        internal static void SaveImage(BitmapSource source, Stream stream)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
        }
    }
}
