using System;
using System.IO;
using System.Reactive;
using ReactiveUI;

namespace ImageSearch.Interactions
{
    public static class ImageInteractions
    {
        public static Interaction<Uri, Unit> SearchWithUri { get; } = new Interaction<Uri, Unit>();
        public static Interaction<FileInfo, Unit> SearchWithFile { get; } = new Interaction<FileInfo, Unit>();
    }
}
