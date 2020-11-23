using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BooruDotNet.Search.WPF.Models;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private const double _bestMatchThreshold = 0.85;
        private SearchServiceModel _selectedService;
        private UploadMethodModel _selectedUploadMethod;
        private readonly UriUploadViewModel _uriUploadViewModel;
        private readonly FileUploadViewModel _fileUploadViewModel;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResults;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsBest;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsOther;
        private readonly ObservableAsPropertyHelper<bool> _hasBestResults;
        private readonly ObservableAsPropertyHelper<bool> _hasOtherResults;
        private readonly ObservableAsPropertyHelper<bool> _isSearching;

        public MainViewModel()
        {
            _uriUploadViewModel = new UriUploadViewModel();
            _fileUploadViewModel = new FileUploadViewModel();

            SearchServices = new[]
            {
                WPF.SearchServices.Danbooru,
                WPF.SearchServices.DanbooruIqdb
            };

            UploadMethods = new[]
            {
                new UploadMethodModel(UploadMethod.Uri, "URL", _uriUploadViewModel),
                new UploadMethodModel(UploadMethod.File, "File", _fileUploadViewModel),
            };

            SearchCommand = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(LoadResultsAsync).TakeUntil(CancelSearchCommand),
                this.WhenAnyValue(
                    x => x.SelectedUploadMethod,
                    x => x._uriUploadViewModel.ImageUri,
                    x => x._fileUploadViewModel.FileInfo,
                    (model, uri, fileInfo) =>
                    {
                        return model?.Method switch
                        {
                            UploadMethod.Uri => uri?.IsAbsoluteUri ?? false,
                            UploadMethod.File => fileInfo?.Exists ?? false,
                            _ => false,
                        };
                    }));

            SearchCommand.ThrownExceptions.Subscribe(ex =>
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));

            CancelSearchCommand = ReactiveCommand.Create(
                () => { },
                SearchCommand.IsExecuting);

            _isSearching = SearchCommand.IsExecuting.ToProperty(this, x => x.IsSearching);

            _searchResults = SearchCommand.ToProperty(
                this,
                x => x.SearchResults,
                initialValue: Enumerable.Empty<ResultViewModel>(),
                scheduler: RxApp.MainThreadScheduler);

            #region Best results

            _searchResultsBest = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(results => results.Where(result => result.Similarity > _bestMatchThreshold))
                .ToProperty(this, x => x.SearchResultsBestMatches);

            _hasBestResults = this
                .WhenAnyValue(x => x.SearchResultsBestMatches)
                .Select(results => results.Any())
                .ToProperty(this, x => x.HasBestResults);

            #endregion

            #region Other results

            _searchResultsOther = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(results => results.Where(result => result.Similarity <= _bestMatchThreshold))
                .ToProperty(this, x => x.SearchResultsOtherMatches);

            _hasOtherResults = this
                .WhenAnyValue(x => x.SearchResultsOtherMatches)
                .Select(results => results.Any())
                .ToProperty(this, x => x.HasOtherResults);

            #endregion
        }

        public SearchServiceModel SelectedService
        {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }

        public UploadMethodModel SelectedUploadMethod
        {
            get => _selectedUploadMethod;
            set => this.RaiseAndSetIfChanged(ref _selectedUploadMethod, value);
        }

        public IEnumerable<ResultViewModel> SearchResults => _searchResults.Value;
        public IEnumerable<ResultViewModel> SearchResultsBestMatches => _searchResultsBest.Value;
        public IEnumerable<ResultViewModel> SearchResultsOtherMatches => _searchResultsOther.Value;
        public bool HasBestResults => _hasBestResults.Value;
        public bool HasOtherResults => _hasOtherResults.Value;
        public bool IsSearching => _isSearching.Value;
        public IEnumerable<SearchServiceModel> SearchServices { get; }
        public IEnumerable<UploadMethodModel> UploadMethods { get; }

        public ReactiveCommand<Unit, IEnumerable<ResultViewModel>> SearchCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelSearchCommand { get; }

        private async Task<IEnumerable<ResultViewModel>> LoadResultsAsync(CancellationToken cancellationToken)
        {
            // Task.Run(...) fixes the command blocking the UI.
            var task = Task
                .Run(async () =>
                {
                    switch (SelectedUploadMethod.Method)
                    {
                        case UploadMethod.Uri:
                            return await SelectedService.SearchByAsync(_uriUploadViewModel.ImageUri, cancellationToken);

                        case UploadMethod.File:
                            using (var fileStream = _fileUploadViewModel.FileInfo.OpenRead())
                            {
                                return await SelectedService.SearchByAsync(fileStream, cancellationToken);
                            }

                        default:
                            throw new InvalidOperationException();
                    }
                }, cancellationToken)
                .ConfigureAwait(false);
            var results = await task;
            return results.Select(r => new ResultViewModel(r));
        }
    }
}
