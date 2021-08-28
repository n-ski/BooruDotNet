using System.IO;
using System.Reactive;
using ReactiveUI;

namespace BooruDownloader.Interactions
{
    public static class DialogInteractions
    {
        public static Interaction<string, FileInfo?> OpenFileBrowser { get; } = new Interaction<string, FileInfo?>();
        public static Interaction<Unit, DirectoryInfo?> OpenFolderBrowser { get; } = new Interaction<Unit, DirectoryInfo?>();
    }
}
