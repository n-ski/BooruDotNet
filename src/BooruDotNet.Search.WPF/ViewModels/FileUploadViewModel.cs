using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using BooruDotNet.Search.WPF.Interactions;
using Humanizer.Bytes;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class FileUploadViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<FileInfo> _fileInfo;

        public FileUploadViewModel()
        {
            OpenFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                FileInfo fileInfo = await DialogInteractions.OpenFileBrowser.Handle("Images|*.jpg;*.jpeg;*.png;*.gif");

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

            _fileInfo = OpenFileCommand.ToProperty(this, x => x.FileInfo);
        }

        public FileInfo FileInfo => _fileInfo.Value;

        public ReactiveCommand<Unit, FileInfo> OpenFileCommand { get; }
    }
}
