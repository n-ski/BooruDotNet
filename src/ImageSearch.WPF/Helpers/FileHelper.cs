using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageSearch.WPF.Helpers
{
    internal static class FileHelper
    {
        public static ImmutableArray<string> ImageFileExtensions = ImmutableArray.Create(
            ".gif",
            ".jpeg",
            ".jpg",
            ".png");

        public static bool IsImageFile(string path)
        {
            Debug.Assert(path is object);

            string fileExt = Path.GetExtension(path);

            if (fileExt.Length is 0)
            {
                return false;
            }

            return ImageFileExtensions.Any(extension => extension.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsImageFile(FileInfo fileInfo)
        {
            Debug.Assert(fileInfo is object);

            string fileExt = fileInfo.Extension;

            if (fileExt.Length is 0)
            {
                return false;
            }

            return ImageFileExtensions.Any(extension => extension.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
        }
    }
}
