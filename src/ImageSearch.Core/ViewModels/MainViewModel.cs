using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private const double _bestResultThreshold = 0.8;

        public MainViewModel()
        {
            StatusViewModel = new StatusViewModel();
            BestSearchResultsViewModel = new SearchResultGroupViewModel();
            OtherSearchResultsViewModel = new SearchResultGroupViewModel();

            SelectedSearchService = SearchServices.First();
            SelectedUploadMethod = UploadMethods.First();

            this.WhenAnyValue(x => x.SelectedUploadMethod, selector: method => method?.Search)
                .ToPropertyEx(this, x => x.Search);

            this.WhenAnyValue(x => x.SelectedUploadMethod)
                .WhereNotNull()
                .Select(method => method.CurrentStatus)
                .Switch()
                .BindTo(this, x => x.StatusViewModel.StatusText);

            this.WhenAnyValue(x => x.Search)
                .WhereNotNull()
                .Select(search => search.IsExecuting)
                .Switch()
                .BindTo(this, x => x.StatusViewModel.IsActive);

            UploadMethods
                .AsObservableChangeSet()
                .SubscribeMany(method => Observable
                    .Return(StatusViewModel.CancelOperation)
                    .BindTo(method, m => m.CancelSearch))
                .Subscribe();

            SearchForSimilar = ReactiveCommand.CreateFromObservable((Uri uri) => SearchForSimilarImpl(uri));

            SearchWithFile = ReactiveCommand.CreateFromObservable((FileInfo file) => SearchWithFileImpl(file));

            // Assume that we can't switch methods while searching.
            this.WhenAnyValue(x => x.Search)
                .WhereNotNull()
                .Select(search => search.ThrownExceptions)
                .Switch()
                .Merge(SearchWithUri.ThrownExceptions)
                .Merge(SearchWithFile.ThrownExceptions)
                .Throttle(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
                .SelectMany(DisplaySearchError.Handle)
                .Subscribe();

            // Whenever file upload is selected, observe the file selection and search with the newly selected file.
            this.WhenAnyValue(x => x.SelectedUploadMethod)
                .WhereNotNull()
                .OfType<FileUploadViewModel>()
                .Select(fileUpload => fileUpload.WhenAnyObservable(f => f.SelectFile))
                .Switch()
                .WhereNotNull()
                .InvokeCommand(this, x => x.SearchWithFile);

            OpenSource = ReactiveCommand.CreateFromObservable((Uri uri) => OpenUriInteraction.Handle(uri));

            CopySource = ReactiveCommand.CreateFromObservable((Uri uri) => CopyUriInteraction.Handle(uri));

            #region Search results bindings

            var searchResults = this.WhenAnyValue(x => x.Search)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .WhereNotNull()
                .Switch()
                .Select(results => results.AsObservableChangeSet())
                .Switch()
                .Filter(result => result.SourceUri is object);

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

            static IDisposable SelectAndSubscribeOnObservable<TIn, TAny, TOut>(
                IObservable<IChangeSet<TIn>> changeSet,
                Func<TIn, IObservable<TAny>> observable,
                Func<TIn, TOut> selector,
                Func<IObservable<TOut>, IDisposable> subscriptionFactory)
            {
                return changeSet.SubscribeMany(item => subscriptionFactory(observable(item).Select(_ => selector(item)))).Subscribe();
            }

            SelectAndSubscribeOnObservable(searchResults, result => result.OpenSource, result => result.SourceUri, uri => uri.InvokeCommand(this, x => x.OpenSource));

            SelectAndSubscribeOnObservable(searchResults, result => result.CopySource, result => result.SourceUri, uri => uri.InvokeCommand(this, x => x.CopySource));

            SelectAndSubscribeOnObservable(searchResults, result => result.SearchForSimilar, result => result.ImageUri, uri => uri.InvokeCommand(this, x => x.SearchForSimilar));

            #endregion
        }

        #region Properties

        public StatusViewModel StatusViewModel { get; }

        public SearchResultGroupViewModel BestSearchResultsViewModel { get; }

        public SearchResultGroupViewModel OtherSearchResultsViewModel { get; }

        public IReadOnlyCollection<UploadViewModelBase> UploadMethods { get; } = new UploadViewModelBase[]
        {
            new FileUploadViewModel(),
            new UriUploadViewModel(),
        };

        [Reactive]
        public UploadViewModelBase? SelectedUploadMethod { get; set; }

        public IEnumerable<SearchServiceViewModel>? SearchServices { get; } = Locator.Current.GetService<IEnumerable<SearchServiceViewModel>>();

        [Reactive]
        public SearchServiceViewModel? SelectedSearchService { get; set; }

        #endregion

        #region Commands

        public ReactiveCommand<Uri, Unit> OpenSource { get; }
        public ReactiveCommand<Uri, Unit> CopySource { get; }
        public ReactiveCommand<Uri, Unit> SearchForSimilar { get; }
        public ReactiveCommand<FileInfo, Unit> SearchWithFile { get; }
        public extern ReactiveCommand<SearchServiceViewModel, IReadOnlyCollection<SearchResultViewModel>>? Search { [ObservableAsProperty] get; }

        #endregion

        #region Interactions

        public Interaction<Uri, Unit> OpenUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Uri, Unit> CopyUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Exception, Unit> DisplaySearchError { get; } = new Interaction<Exception, Unit>();

        #endregion

        #region Command implementations

        private IObservable<Unit> SearchForSimilarImpl(Uri uri)
        {
            var uriUpload = UploadMethods.OfType<UriUploadViewModel>().Single();
            uriUpload.FileUri = uri;

            SelectedUploadMethod = uriUpload;

            return uriUpload.Search.Execute(SelectedSearchService!).Select(_ => Unit.Default);
        }

        private IObservable<Unit> SearchWithFileImpl(FileInfo fileInfo)
        {
            var fileUpload = UploadMethods.OfType<FileUploadViewModel>().Single();
            fileUpload.FileToUpload = fileInfo;

            SelectedUploadMethod = fileUpload;

            return fileUpload.Search.Execute(SelectedSearchService!).Select(_ => Unit.Default);
        }

        #endregion
    }
}
