using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ImageSearch.WPF.Helpers
{
    internal static class DropDataHelper
    {
        public static FileInfo GetFirstDroppedFile(DataObject dataObject)
        {
            Debug.Assert(dataObject is object);
            Debug.Assert(dataObject.ContainsFileDropList());

            string firstFileName = dataObject.GetFileDropList()[0];
            return new FileInfo(firstFileName);
        }
    }
}
