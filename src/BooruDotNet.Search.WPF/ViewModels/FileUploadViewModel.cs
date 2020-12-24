using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using BooruDotNet.Search.WPF.Helpers;
using BooruDotNet.Search.WPF.Interactions;
using Humanizer.Bytes;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class FileUploadViewModel : ReactiveObject
    {
        private FileInfo _fileInfo;

        public FileUploadViewModel()
        {
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                FileInfo fileInfo = await DialogInteractions.OpenFileBrowser.Handle(FileHelper.OpenFileDialogFilter);

                if (fileInfo.Length > 8 * ByteSize.BytesInMegabyte)
                {
                    await MessageInteractions.Warning.Handle("File exceeds maximum file size of 8 MB.");
                    return FileInfo;
                }
                else
                {
                    return fileInfo;
                }
            });

            OpenFileCommand.Subscribe(file => FileInfo = file);
        }

        public FileInfo FileInfo
        {
            get => _fileInfo;
            set => this.RaiseAndSetIfChanged(ref _fileInfo, value);
        }

        public ReactiveCommand<Unit, FileInfo> OpenFileCommand { get; }
    }
}
