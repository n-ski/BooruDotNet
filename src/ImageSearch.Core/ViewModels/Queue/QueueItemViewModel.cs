using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
            CurrentStatusSubject = new Subject<string>();
            CurrentStatusSubject.ToPropertyEx(this, x => x.CurrentStatus);

            Search = ReactiveCommand.CreateFromObservable(
                (IFileAndUriSearchService service) => Observable.StartAsync(ct => SearchImpl(service, ct)).TakeUntil(CancelSearch!));

            Search
                .Where(items => items.Any())
                .Select(items => $"Found {items.Count()} results.")
                .Subscribe(text => CurrentStatusSubject.OnNext(text));

            Search.ThrownExceptions
                .Subscribe(ex => CurrentStatusSubject.OnNext(ex.Message));

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

        public extern IBitmap? Thumbnail { [ObservableAsProperty] get; }

        public extern string? CurrentStatus { [ObservableAsProperty] get; }

        public ReadOnlyObservableCollection<SearchResultViewModel> SearchResults { get; }

        protected Subject<string> CurrentStatusSubject { get; }

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
