﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private const double _bestResultThreshold = 0.8;
        private readonly FileUploadViewModel _fileUploadViewModel;
        private readonly UriUploadViewModel _uriUploadViewModel;

        public MainViewModel()
        {
            StatusViewModel = new StatusViewModel();
            BestSearchResultsViewModel = new SearchResultGroupViewModel();
            OtherSearchResultsViewModel = new SearchResultGroupViewModel();
            _fileUploadViewModel = new FileUploadViewModel();
            _uriUploadViewModel = new UriUploadViewModel();

            UploadMethods = (UploadMethod[])Enum.GetValues(typeof(UploadMethod));
            SearchServices = Locator.Current.GetService<IEnumerable<SearchServiceViewModel>>();

            var canSearch = this.WhenAnyValue(
                x => x.UploadMethod,
                x => x._fileUploadViewModel.FileToUpload,
                x => x._uriUploadViewModel.FileUri,
                x => x.SelectedSearchService,
                (selectedMethod, file, uri, selectedService) =>
                {
                    if (selectedService is object)
                    {
                        switch (selectedMethod)
                        {
                            case FileUploadViewModel _:
                                return file?.Exists is true;

                            case UriUploadViewModel _:
                                return uri?.IsAbsoluteUri is true;
                        }
                    }

                    return false;
                });

            Search = ReactiveCommand.CreateFromObservable(() => Observable.StartAsync(SearchImpl).TakeUntil(StatusViewModel.CancelOperation), canSearch);

            Search.IsExecuting.BindTo(this, x => x.StatusViewModel.IsActive);

            SearchForSimilar = ReactiveCommand.CreateFromObservable<Uri, Unit>(SearchForSimilarImpl);

            Search.ThrownExceptions.Merge(SearchForSimilar.ThrownExceptions)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .Subscribe(async ex => await DisplaySearchError.Handle(ex));

            OpenSource = ReactiveCommand.CreateFromObservable<Uri, Unit>(uri => OpenUriInteraction.Handle(uri));

            CopySource = ReactiveCommand.CreateFromObservable<Uri, Unit>(uri => CopyUriInteraction.Handle(uri));

            this.WhenAnyValue(x => x.SelectedUploadMethod)
                .WhereNotNull()
                .Select<UploadMethod?, UploadViewModelBase>(method => method switch
                {
                    ImageSearch.UploadMethod.File => _fileUploadViewModel,
                    ImageSearch.UploadMethod.Uri => _uriUploadViewModel,
                    _ => throw Assumes.NotReachable()
                }).ToPropertyEx(this, x => x.UploadMethod);

            this.WhenAnyObservable(x => x.UploadMethod!.Search)
                .InvokeCommand(this, x => x.Search);

            #region Search results bindings

            var searchResults = Search
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(results => results.AsObservableChangeSet())
                .Switch()
                .Filter(result => result.SourceUri is object)
                .DisposeMany();

            searchResults
                .Filter(result => result.Similarity >= _bestResultThreshold)
                .ToCollection()
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, x => x.BestSearchResultsViewModel.SearchResults);

            searchResults
                .Filter(result => result.Similarity < _bestResultThreshold)
                .ToCollection()
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, x => x.OtherSearchResultsViewModel.SearchResults);

            // Observe commands from search results and pipe them to the main commands.

            static IObservable<TOut> SelectOnObservable<TIn, TAny, TOut>(
                IObservable<IChangeSet<TIn>> changeSet,
                Func<TIn, IObservable<TAny>> observable,
                Func<TIn, TOut> selector)
            {
                return changeSet
                    .AutoRefreshOnObservable(observable)
                    .Where(set => set.Refreshes is 1)
                    .Select(set => selector(set.First().Item.Current));
            }

            SelectOnObservable(searchResults, result => result.OpenSource, result => result.SourceUri)
                .InvokeCommand(this, x => x.OpenSource);

            SelectOnObservable(searchResults, result => result.CopySource, result => result.SourceUri)
                .InvokeCommand(this, x => x.CopySource);

            SelectOnObservable(searchResults, result => result.SearchForSimilar, result => result.ImageUri)
                .InvokeCommand(this, x => x.SearchForSimilar);

            #endregion
        }

        #region Properties

        public StatusViewModel StatusViewModel { get; }

        public SearchResultGroupViewModel BestSearchResultsViewModel { get; }

        public SearchResultGroupViewModel OtherSearchResultsViewModel { get; }

        public IEnumerable<UploadMethod> UploadMethods { get; }

        [Reactive]
        public UploadMethod? SelectedUploadMethod { get; set; }

        public IEnumerable<SearchServiceViewModel>? SearchServices { get; }

        [Reactive]
        public SearchServiceViewModel? SelectedSearchService { get; set; }

        public extern UploadViewModelBase? UploadMethod { [ObservableAsProperty] get; }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, IReadOnlyCollection<SearchResultViewModel>> Search { get; }
        public ReactiveCommand<Uri, Unit> OpenSource { get; }
        public ReactiveCommand<Uri, Unit> CopySource { get; }
        public ReactiveCommand<Uri, Unit> SearchForSimilar { get; }

        #endregion

        #region Interactions

        public Interaction<Uri, Unit> OpenUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Uri, Unit> CopyUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Exception, Unit> DisplaySearchError { get; } = new Interaction<Exception, Unit>();

        #endregion

        #region Command implementations

        private async Task<IReadOnlyCollection<SearchResultViewModel>> SearchImpl(CancellationToken cancellationToken)
        {
            Debug.Assert(UploadMethod is object);
            Debug.Assert(SelectedSearchService is object);

            IEnumerable<IResult> results;

            // TODO: Store localizable text in resources.
            StatusViewModel.StatusText = "Please wait\u2026";

            switch (UploadMethod)
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

            return results.Select(x => new SearchResultViewModel(x)).ToArray();
        }

        private IObservable<Unit> SearchForSimilarImpl(Uri uri)
        {
            SelectedUploadMethod = ImageSearch.UploadMethod.Uri;

            if (UploadMethod is UriUploadViewModel uriUpload)
            {
                uriUpload.FileUri = uri;
            }
            else
            {
                throw Assumes.NotReachable();
            }

            return Search.Execute().Select(_ => Unit.Default);
        }

        #endregion
    }
}
