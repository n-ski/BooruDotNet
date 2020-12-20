using System.IO;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Interactions
{
    public static class DialogInteractions
    {
        public static Interaction<string, FileInfo> OpenFileBrowser { get; } = new Interaction<string, FileInfo>();
    }
}
