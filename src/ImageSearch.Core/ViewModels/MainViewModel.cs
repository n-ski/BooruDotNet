using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;
using DynamicData;
using ImageSearch.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Validation;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private const double _bestResultThreshold = 0.8;
        private readonly ReadOnlyObservableCollection<SearchResultViewModel> _bestResults;
        private readonly ReadOnlyObservableCollection<SearchResultViewModel> _otherResults;

        public MainViewModel()
        {
            UploadMethods = new UploadViewModelBase[]
            {
                new FileUploadViewModel(),
                new UriUploadViewModel(),
            };

            var canSearch = this.WhenAnyValue(
                x => x.SelectedUploadMethod,
                x => x.SelectedSearchService,
                (selectedMethod, selectedService) =>
                {
                    if (selectedService is object)
                    {
                        switch (selectedMethod)
                        {
                            case FileUploadViewModel fileUpload:
                                return fileUpload.FileToUpload?.Exists is true;

                            case UriUploadViewModel uriUpload:
                                return uriUpload.FileUri?.IsAbsoluteUri is true;
                        }
                    }

                    return false;
                });

            Search = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(SearchImpl).TakeUntil(CancelSearch!), canSearch);

            Search.IsExecuting.ToPropertyEx(this, x => x.IsSearching);

            SearchForSimilar = ReactiveCommand.CreateFromObservable<Uri, IEnumerable<SearchResultViewModel>>(SearchForSimilarImpl, canSearch);

            CancelSearch = ReactiveCommand.Create(MethodHelper.DoNothing, Search.IsExecuting);

            OpenSource = ReactiveCommand.CreateFromObservable<Uri, Unit>(uri => OpenUriInteraction.Handle(uri));

            CopySource = ReactiveCommand.CreateFromObservable<Uri, Unit>(uri => CopyUriInteraction.Handle(uri));

            this.WhenAnyObservable(x => x.SelectedUploadMethod!.Search)
                .InvokeCommand(this, x => x.Search);

            #region Search results bindings

            // Not sure whether or not we're supposed to dispose of these subscriptions.
            // MainViewModel doesn't change so it shouldn't be a problem.

            var searchResults = Search
                .ToObservableChangeSet()
                .RefCount();

            searchResults
                .Transform(result => result
                    .WhenAnyObservable(r => r.OpenSource)
                    .Select(_ => result.SourceUri)
                    .InvokeCommand(this, x => x.OpenSource))
                .DisposeMany()
                .Subscribe();

            searchResults
                .Transform(result => result
                    .WhenAnyObservable(r => r.CopySource)
                    .Select(_ => result.SourceUri)
                    .InvokeCommand(this, x => x.CopySource))
                .DisposeMany()
                .Subscribe();

            searchResults
                .Transform(result => result
                    .WhenAnyObservable(r => r.SearchForSimilar)
                    .Select(_ => result.SourceUri)
                    .InvokeCommand(this, x => x.SearchForSimilar))
                .DisposeMany()
                .Subscribe();

            searchResults
                .Filter(result => result.Similarity >= _bestResultThreshold)
                .Bind(out _bestResults)
                .DisposeMany()
                .Subscribe();

            searchResults
                .Filter(result => result.Similarity < _bestResultThreshold)
                .Bind(out _otherResults)
                .DisposeMany()
                .Subscribe();

            #endregion
        }

        #region Properties

        public IEnumerable<UploadViewModelBase> UploadMethods { get; }

        [Reactive]
        public UploadViewModelBase? SelectedUploadMethod { get; set; }

        [Reactive]
        public IEnumerable<SearchServiceViewModel>? SearchServices { get; set; }

        [Reactive]
        public SearchServiceViewModel? SelectedSearchService { get; set; }

        public extern bool IsSearching { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<SearchResultViewModel> BestResults => _bestResults;

        public ReadOnlyObservableCollection<SearchResultViewModel> OtherResults => _otherResults;

        #endregion

        #region Commands

        public ReactiveCommand<Unit, IEnumerable<SearchResultViewModel>> Search { get; }
        public ReactiveCommand<Unit, Unit> CancelSearch { get; }
        public ReactiveCommand<Uri, Unit> OpenSource { get; }
        public ReactiveCommand<Uri, Unit> CopySource { get; }
        public ReactiveCommand<Uri, IEnumerable<SearchResultViewModel>> SearchForSimilar { get; }

        #endregion

        #region Interactions

        public Interaction<Uri, Unit> OpenUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Uri, Unit> CopyUriInteraction { get; } = new Interaction<Uri, Unit>();

        #endregion

        #region Command implementations

        private async Task<IEnumerable<SearchResultViewModel>> SearchImpl(CancellationToken cancellationToken)
        {
            Debug.Assert(SelectedUploadMethod is object);
            Debug.Assert(SelectedSearchService is object);

            IEnumerable<IResult> results;

            switch (SelectedUploadMethod)
            {
                case FileUploadViewModel fileUpload:
                {
                    Debug.Assert(fileUpload.FileToUpload is object);

                    using var fileStream = fileUpload.FileToUpload.OpenRead();
                    results = await SelectedSearchService.SearchAsync(fileStream, cancellationToken);
                    break;
                }

                case UriUploadViewModel uriUpload:
                {
                    Debug.Assert(uriUpload.FileUri is object);

                    results = await SelectedSearchService.SearchAsync(uriUpload.FileUri, cancellationToken);
                    break;
                }

                default:
                    throw Assumes.NotReachable();
            }

            return results.Select(x => new SearchResultViewModel(x));
        }

        private IObservable<IEnumerable<SearchResultViewModel>> SearchForSimilarImpl(Uri uri)
        {
            var uriUpload = UploadMethods.OfType<UriUploadViewModel>().First();
            uriUpload.FileUri = uri;

            SelectedUploadMethod = uriUpload;

            return Search.Execute();
        }

        #endregion
    }
}
