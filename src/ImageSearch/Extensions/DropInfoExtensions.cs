using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ImageSearch.Extensions
{
    internal static class DropInfoExtensions
    {
        internal static bool TryGetDroppedFiles(this IDropInfo dropInfo, [NotNullWhen(true)] out IEnumerable<string>? files)
        {
            if (dropInfo.Data is DataObject data && data.ContainsFileDropList())
            {
                files = data.GetFileDropList().Cast<string>();
                return true;
            }
            else
            {
                files = null;
                return false;
            }
        }

        internal static bool TryGetDroppedText(this IDropInfo dropInfo, [NotNullWhen(true)] out string? text)
        {
            if (dropInfo.Data is DataObject data && data.ContainsText())
            {
                text = data.GetText().Trim();
                return true;
            }
            else
            {
                text = null;
                return false;
            }
        }
    }
}
