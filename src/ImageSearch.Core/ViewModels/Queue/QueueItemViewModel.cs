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
                .Select(items => $"Found {items.Count()} results.")
                .BindTo(StatusViewModel, s => s.Text);

            Search
                .Select(_ => QueueItemStatus.Complete)
                .BindTo(StatusViewModel, s => s.Status);

            Search.ThrownExceptions
                .BindTo(StatusViewModel, s => s.Exception);

            CancelSearch = ReactiveCommand.Create(
                MethodHelper.DoNothing,
                this.WhenAnyObservable(x => x.Search.IsExecuting));

            var searchResults = Search
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(results => results.AsObservableChangeSet())
                .Switch()
                .Filter(result => result.SourceUri is object)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var bestResults)
                .Subscribe();

            SearchResults = bestResults;

            LoadThumbnail = ReactiveCommand.CreateFromTask(LoadThumbnailImpl);

            LoadThumbnail.ToPropertyEx(this, x => x.Thumbnail);

            RemoveItem = ReactiveCommand.Create(MethodHelper.DoNothing);
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

        public ReactiveCommand<Unit, Unit> RemoveItem { get; }

        #endregion

        protected abstract Task<IBitmap> LoadThumbnailImpl();

        protected abstract Task<IEnumerable<SearchResultViewModel>> SearchImpl(IFileAndUriSearchService service, CancellationToken ct);
    }
}
