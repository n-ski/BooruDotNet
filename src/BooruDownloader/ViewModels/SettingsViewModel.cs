using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace BooruDownloader.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private int _batchSize;
        private bool _ignoreDownloadErrors;
        private FileNamingStyle _fileNamingStyle;
        private bool _ignoreArchiveFiles;
        private bool _notifyAboutSkippedPosts;
        private bool _playSoundWhenComplete;
        private bool _overwirteExistingFiles;
        private bool _askLocationBeforeDownload;
        private string _downloadLocation; // I'd really love to make this a OAPH, but we need to read from the settings into the property.
        private static readonly IEnumerable<FileNamingStyle> _fileNamingStyles = Enum.GetValues(typeof(FileNamingStyle)).Cast<FileNamingStyle>();

        public SettingsViewModel()
        {
            LoadSettings();

            ChangeDownloadLocation = ReactiveCommand.CreateFromObservable(
                () => Interactions.OpenFolderBrowser.Handle(Unit.Default),
                this.WhenAnyValue(x => x.AskLocationBeforeDownload, ask => !ask));

            ChangeDownloadLocation.Subscribe(directoryInfo =>
            {
                if (directoryInfo != null)
                {
                    DownloadLocation = directoryInfo.FullName;
                }
            });

            SaveSettings = ReactiveCommand.Create(SaveSettingsImpl);
        }

        public int BatchSize
        {
            get => _batchSize;
            set => this.RaiseAndSetIfChanged(ref _batchSize, Math.Max(1, value));
        }

        public bool IgnoreDownloadErrors
        {
            get => _ignoreDownloadErrors;
            set => this.RaiseAndSetIfChanged(ref _ignoreDownloadErrors, value);
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

        public bool NotifyAboutSkippedPosts
        {
            get => _notifyAboutSkippedPosts;
            set => this.RaiseAndSetIfChanged(ref _notifyAboutSkippedPosts, value);
        }

        public bool PlaySoundWhenComplete
        {
            get => _playSoundWhenComplete;
            set => this.RaiseAndSetIfChanged(ref _playSoundWhenComplete, value);
        }

        public bool OverwriteExistingFiles
        {
            get => _overwirteExistingFiles;
            set => this.RaiseAndSetIfChanged(ref _overwirteExistingFiles, value);
        }

        public bool AskLocationBeforeDownload
        {
            get => _askLocationBeforeDownload;
            set => this.RaiseAndSetIfChanged(ref _askLocationBeforeDownload, value);
        }

        public string DownloadLocation
        {
            get => _downloadLocation;
            private set => this.RaiseAndSetIfChanged(ref _downloadLocation, value);
        }

        public IEnumerable<FileNamingStyle> FileNamingStyles => _fileNamingStyles;

        public ReactiveCommand<Unit, DirectoryInfo> ChangeDownloadLocation { get; }

        public ReactiveCommand<Unit, Unit> SaveSettings { get; }

        private void LoadSettings()
        {
            var settings = Settings.Default;

            BatchSize = settings.BatchSize;
            IgnoreDownloadErrors = settings.IgnoreDownloadErrors;
            FileNamingStyle = settings.FileNamingStyle;
            IgnoreArchiveFiles = settings.IgnoreArchiveFiles;
            NotifyAboutSkippedPosts = settings.NotifyAboutSkippedPosts;
            PlaySoundWhenComplete = settings.PlaySoundWhenComplete;
            OverwriteExistingFiles = settings.OverwriteExistingFiles;
            AskLocationBeforeDownload = settings.AskLocationBeforeDownload;
            DownloadLocation = Directory.Exists(settings.DownloadLocation)
                ? settings.DownloadLocation
                : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // TODO: needs to be Downloads directory.
        }

        private void SaveSettingsImpl()
        {
            var settings = Settings.Default;

            settings.BatchSize = BatchSize;
            settings.IgnoreDownloadErrors = IgnoreDownloadErrors;
            settings.FileNamingStyle = FileNamingStyle;
            settings.IgnoreArchiveFiles = IgnoreArchiveFiles;
            settings.NotifyAboutSkippedPosts = NotifyAboutSkippedPosts;
            settings.PlaySoundWhenComplete = PlaySoundWhenComplete;
            settings.OverwriteExistingFiles = OverwriteExistingFiles;
            settings.AskLocationBeforeDownload = AskLocationBeforeDownload;
            settings.DownloadLocation = DownloadLocation;

            settings.Save();
        }
    }
}
