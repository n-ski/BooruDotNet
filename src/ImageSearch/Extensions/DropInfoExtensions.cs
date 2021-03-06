using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ImageSearch.Extensions
{
    internal static class DropInfoExtensions
    {
        internal static bool TryGetDroppedFiles(this IDropInfo dropInfo, out IEnumerable<string> files)
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

        internal static bool TryGetDroppedText(this IDropInfo dropInfo, out string text)
        {
            if (dropInfo.Data is DataObject data && data.ContainsText())
            {
                text = data.GetText();
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
