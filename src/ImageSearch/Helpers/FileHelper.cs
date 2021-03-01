using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

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
    }
}
