﻿using System;
using System.IO;
using System.Reactive;
using ReactiveUI;

namespace BooruDotNet.Downloader
{
    public static class Interactions
    {
        public static Interaction<string, FileInfo> OpenFileBrowser { get; } = new Interaction<string, FileInfo>();
        public static Interaction<Unit, DirectoryInfo> OpenFolderBrowser { get; } = new Interaction<Unit, DirectoryInfo>();
        public static Interaction<Exception, Unit> ShowErrorMessage { get; } = new Interaction<Exception, Unit>();
    }
}