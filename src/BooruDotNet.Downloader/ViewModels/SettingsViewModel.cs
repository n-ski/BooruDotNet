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
        // Set an arbitrary limit for now.
        // TODO: re-implement with IntegerUpDown.
        private static readonly IEnumerable<int> _batchSizes = Enumerable.Range(1, 6).ToArray();

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

        public IEnumerable<int> BatchSizes => _batchSizes;

        public ReactiveCommand<Unit, Unit> SaveSettings { get; }

        private void LoadSettings()
        {
            var settings = Settings.Default;

            BatchSize = Math.Clamp(settings.BatchSize, _batchSizes.Min(), _batchSizes.Max());
        }

        private void SaveSettingsImpl()
        {
            var settings = Settings.Default;

            settings.BatchSize = BatchSize;

            settings.Save();
        }
    }
}
