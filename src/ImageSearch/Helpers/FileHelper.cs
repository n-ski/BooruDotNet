using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ImageSearch.Helpers
{
    internal static class FileHelper
    {
        public static ReadOnlyCollection<string> ValidFileExtensions { get; } =
            new ReadOnlyCollection<string>(new[] { ".jpg", ".jpeg", ".png", ".gif" });

        public static string OpenFileDialogFilter = $"Images|*" + string.Join(";*", ValidFileExtensions);

        public static bool IsFileValid(string path)
        {
            var extension = Path.GetExtension(path);

            return ValidFileExtensions.Any(ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public static FileInfo SaveTempImage(BitmapSource bitmapSource)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            var tempFile = new FileInfo(Path.Combine(Path.GetTempPath(), "tempupload.png"));

            using (Stream stream = tempFile.Create())
            {
                encoder.Save(stream);
            }

            tempFile.Refresh();
            return tempFile;
        }
    }
}
