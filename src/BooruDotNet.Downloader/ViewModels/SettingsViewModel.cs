using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private int _batchSize;
        private FileNamingStyle _fileNamingStyle;
        private bool _ignoreArchiveFiles;
        // Set an arbitrary limit for now.
        // TODO: re-implement with IntegerUpDown.
        private static readonly IEnumerable<int> _batchSizes = Enumerable.Range(1, 6).ToArray();
        private static readonly IEnumerable<FileNamingStyle> _fileNamingStyles = Enum.GetValues(typeof(FileNamingStyle)).Cast<FileNamingStyle>();

        public SettingsViewModel()
        {
            LoadSettings();

            SaveSettings = ReactiveCommand.Create(SaveSettingsImpl);
        }

        public int BatchSize
        {
            get => _batchSize;
            set => this.RaiseAndSetIfChanged(ref _batchSize, value);
        }

        public FileNamingStyle FileNamingStyle
        {
            get => _fileNamingStyle;
            set => this.RaiseAndSetIfChanged(ref _fileNamingStyle, value);
        }

        public bool IgnoreArchiveFiles
        {
            get => _ignoreArchiveFiles;
            set => this.RaiseAndSetIfChanged(ref _ignoreArchiveFiles, value);
        }

        public IEnumerable<int> BatchSizes => _batchSizes;

        public IEnumerable<FileNamingStyle> FileNamingStyles => _fileNamingStyles;

        public ReactiveCommand<Unit, Unit> SaveSettings { get; }

        private void LoadSettings()
        {
            var settings = Settings.Default;

            BatchSize = Math.Clamp(settings.BatchSize, _batchSizes.Min(), _batchSizes.Max());
            FileNamingStyle = settings.FileNamingStyle;
            IgnoreArchiveFiles = settings.IgnoreArchiveFiles;
        }

        private void SaveSettingsImpl()
        {
            var settings = Settings.Default;

            settings.BatchSize = BatchSize;
            settings.FileNamingStyle = FileNamingStyle;
            settings.IgnoreArchiveFiles = IgnoreArchiveFiles;

            settings.Save();
        }
    }
}
