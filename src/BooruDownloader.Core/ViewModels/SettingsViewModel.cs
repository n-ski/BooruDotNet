using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using BooruDownloader.Interactions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BooruDownloader.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private static readonly IEnumerable<FileNamingStyle> _fileNamingStyles = Enum.GetValues(typeof(FileNamingStyle)).Cast<FileNamingStyle>();

        public SettingsViewModel()
        {
            LoadSettings();

            ChangeDownloadLocation = ReactiveCommand.CreateFromObservable(
                () => DialogInteractions.OpenFolderBrowser.Handle(Unit.Default),
                this.WhenAnyValue(x => x.AskLocationBeforeDownload, ask => ask is false));

            ChangeDownloadLocation
                .WhereNotNull()
                .Select(d => d.FullName)
                .BindTo(this, x => x.DownloadLocation);

            SaveSettings = ReactiveCommand.Create(SaveSettingsImpl);
        }

        [Reactive]
        public int BatchSize { get; set; }

        [Reactive]
        public bool IgnoreDownloadErrors { get; set; }

        [Reactive]
        public FileNamingStyle FileNamingStyle { get; set; }

        [Reactive]
        public bool IgnoreArchiveFiles { get; set; }

        [Reactive]
        public bool NotifyAboutSkippedPosts { get; set; }

        [Reactive]
        public bool PlaySoundWhenComplete { get; set; }

        [Reactive]
        public bool OverwriteExistingFiles { get; set; }

        [Reactive]
        public bool AskLocationBeforeDownload { get; set; }

        [Reactive]
        public string? DownloadLocation { get; set; }

        public IEnumerable<FileNamingStyle> FileNamingStyles => _fileNamingStyles;

        public ReactiveCommand<Unit, DirectoryInfo?> ChangeDownloadLocation { get; }

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
