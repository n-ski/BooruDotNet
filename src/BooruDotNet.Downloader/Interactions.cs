using System.IO;
using ReactiveUI;

namespace BooruDotNet.Downloader
{
    public static class Interactions
    {
        public static Interaction<string, FileInfo> OpenFileBrowser { get; } = new Interaction<string, FileInfo>();
    }
}
