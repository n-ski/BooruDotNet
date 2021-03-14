using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ImageSearch.Helpers;
using ImageSearch.Interactions;
using ReactiveUI;

namespace ImageSearch.ViewModels
{
    public class FileUploadViewModel : UploadViewModelBase
    {
        private FileInfo _fileInfo;

        public FileUploadViewModel(string name, UploadMethod uploadMethod)
            : base(name, uploadMethod)
        {
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                FileInfo fileInfo = await DialogInteractions.OpenFileBrowser.Handle(FileHelper.OpenFileDialogFilter);

                // Return old file info if user didn't select any file or if file size exceeds 8 MiB.
                if (fileInfo is null)
                {
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
