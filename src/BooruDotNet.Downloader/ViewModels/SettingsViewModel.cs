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
        private bool _notifyAboutSkippedPosts;
        private bool _playSoundWhenComplete;
        private static readonly IEnumerable<FileNamingStyle> _fileNamingStyles = Enum.GetValues(typeof(FileNamingStyle)).Cast<FileNamingStyle>();

        public SettingsViewModel()
        {
            LoadSettings();

            SaveSettings = ReactiveCommand.Create(SaveSettingsImpl);
        }

        public int BatchSize
        {
            get => _batchSize;
            set => this.RaiseAndSetIfChanged(ref _batchSize, Math.Max(1, value));
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

        public IEnumerable<FileNamingStyle> FileNamingStyles => _fileNamingStyles;

        public ReactiveCommand<Unit, Unit> SaveSettings { get; }

        private void LoadSettings()
        {
            var settings = Settings.Default;

            BatchSize = settings.BatchSize;
            FileNamingStyle = settings.FileNamingStyle;
            IgnoreArchiveFiles = settings.IgnoreArchiveFiles;
            NotifyAboutSkippedPosts = settings.NotifyAboutSkippedPosts;
            PlaySoundWhenComplete = settings.PlaySoundWhenComplete;
        }

        private void SaveSettingsImpl()
        {
            var settings = Settings.Default;

            settings.BatchSize = BatchSize;
            settings.FileNamingStyle = FileNamingStyle;
            settings.IgnoreArchiveFiles = IgnoreArchiveFiles;
            settings.NotifyAboutSkippedPosts = NotifyAboutSkippedPosts;
            settings.PlaySoundWhenComplete = PlaySoundWhenComplete;

            settings.Save();
        }
    }
}
