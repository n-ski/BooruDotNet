using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Validation;

namespace ImageSearch.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        public SettingsViewModel(ApplicationSettings settings)
        {
            Requires.NotNull(settings, nameof(settings));

            EnableFiltering = settings.EnableFiltering;
            MinSimilarity = settings.MinSimilarity;
        }

        [Reactive]
        public bool EnableFiltering { get; set; }

        [Reactive]
        public double MinSimilarity { get; set; }
    }
}
