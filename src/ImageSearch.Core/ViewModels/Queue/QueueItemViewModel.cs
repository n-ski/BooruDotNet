using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using DynamicData;
using ImageSearch.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ImageSearch.ViewModels
{
    public abstract class QueueItemViewModel : ReactiveObject
    {
        protected const float DesiredThumbnailHeight = 50;

        protected QueueItemViewModel()
        {
            StatusViewModel = new QueueItemStatusViewModel();

            Search = ReactiveCommand.CreateFromObservable(
                (IFileAndUriSearchService service) => Observable.StartAsync(ct => SearchImpl(service, ct)).TakeUntil(CancelSearch!));

            Search.IsExecuting
                .Where(isExecuting => isExecuting is true)
                .Select(_ => QueueItemStatus.Processing)
                .BindTo(StatusViewModel, s => s.Status);

            Search
                .Select(_ => QueueItemStatus.Complete)
                .BindTo(StatusViewModel, s => s.Status);

            Search.ThrownExceptions
                .BindTo(StatusViewModel, s => s.Exception);

            CancelSearch = ReactiveCommand.Create(
                MethodHelper.DoNothing,
                this.WhenAnyObservable(x => x.Search.IsExecuting));

            Search
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(results => results.AsObservableChangeSet())
                .Switch()
                // If enabled, filter results based on their similarity.
                .Filter(result => ApplicationSettings.Default.EnableFiltering is false
                    || result.Similarity >= ApplicationSettings.Default.MinSimilarity)
                .Filter(result => result.SourceUri is object)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var searchResults)
                .Subscribe();

            SearchResults = searchResults;

            this.WhenAnyValue(
                x => x.StatusViewModel.Status,
                x => x.SearchResults.Count)
                .Where(tuple => tuple.Item1 is QueueItemStatus.Complete)
                .Select(tuple => $"Found {tuple.Item2} results.")
                .BindTo(StatusViewModel, s => s.Text);

            LoadThumbnail = ReactiveCommand.CreateFromTask(LoadThumbnailImpl);

            LoadThumbnail.ToPropertyEx(this, x => x.Thumbnail);
        }

        #region Properties

        public QueueItemStatusViewModel StatusViewModel { get; }

        public extern IBitmap? Thumbnail { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<SearchResultViewModel> SearchResults { get; }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, IBitmap> LoadThumbnail { get; }

        public ReactiveCommand<IFileAndUriSearchService, IEnumerable<SearchResultViewModel>> Search { get; }

        public ReactiveCommand<Unit, Unit> CancelSearch { get; }

        #endregion

        protected abstract Task<IBitmap> LoadThumbnailImpl();

        protected abstract Task<IEnumerable<SearchResultViewModel>> SearchImpl(IFileAndUriSearchService service, CancellationToken ct);
    }
}
