using System;
using System.IO;
using System.Reactive;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class FileUploadViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<FileInfo> _fileInfo;
        private string _lastDirectory;

        public FileUploadViewModel()
        {
            _lastDirectory = Environment.CurrentDirectory;

            OpenFileCommand = ReactiveCommand.Create(() =>
            {
                if (!Directory.Exists(_lastDirectory))
                {
                    _lastDirectory = Environment.CurrentDirectory;
                }

                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Images|*.jpg;*.jpeg;*.png;*.gif",
                    InitialDirectory = _lastDirectory,
                };

                if (dialog.ShowDialog() == true)
                {
                    var fileInfo = new FileInfo(dialog.FileName);
                    _lastDirectory = fileInfo.DirectoryName;

                    return fileInfo;
                }
                // Return the previously selected file.
                else
                {
                    return FileInfo;
                }
            });

            _fileInfo = OpenFileCommand.ToProperty(this, x => x.FileInfo);
        }

        public FileInfo FileInfo => _fileInfo.Value;

        public ReactiveCommand<Unit, FileInfo> OpenFileCommand { get; }
    }
}
