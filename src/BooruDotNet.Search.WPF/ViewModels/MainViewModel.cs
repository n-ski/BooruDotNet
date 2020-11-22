using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using BooruDotNet.Search.Services;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IqdbService _iqdbService;
        private Uri _searchUri;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResults;

        public MainViewModel()
        {
            _iqdbService = new IqdbService(App.HttpClient, "danbooru");

            SearchCommand = ReactiveCommand.CreateFromTask(
                // Task.Run(...) fixes the command blocking the UI.
                () => Task.Run(LoadResultsAsync),
                this.WhenAnyValue(x => x.SearchUri, uri => uri?.IsAbsoluteUri ?? false));

            _searchResults = SearchCommand.ToProperty(this, x => x.SearchResults, scheduler: RxApp.MainThreadScheduler);

            SearchCommand.ThrownExceptions.Subscribe(ex =>
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));
        }

        public Uri SearchUri
        {
            get => _searchUri;
            set => this.RaiseAndSetIfChanged(ref _searchUri, value);
        }

        public IEnumerable<ResultViewModel> SearchResults => _searchResults.Value;

        public ReactiveCommand<Unit, IEnumerable<ResultViewModel>> SearchCommand { get; }

        private async Task<IEnumerable<ResultViewModel>> LoadResultsAsync()
        {
            var results = await _iqdbService.SearchByAsync(SearchUri).ConfigureAwait(false);
            return results.Select(r => new ResultViewModel(r));
        }
    }
}
