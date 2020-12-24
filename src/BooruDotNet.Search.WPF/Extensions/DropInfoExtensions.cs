using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace BooruDotNet.Search.WPF.Extensions
{
    internal static class DropInfoExtensions
    {
        public static bool TryGetDroppedFiles(this IDropInfo dropInfo, out IEnumerable<string> files)
        {
            if (dropInfo.Data is DataObject data)
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
    }
}
