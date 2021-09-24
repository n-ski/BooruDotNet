using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly SourceList<QueueItemViewModel> _itemsQueue;

        public MainViewModel()
        {
            _itemsQueue = new SourceList<QueueItemViewModel>();

            SelectedSearchService = SearchServices.First();

            SearchWithUri = ReactiveCommand.CreateFromObservable((Uri uri) => SearchWithUriImpl(uri));

            SearchWithFile = ReactiveCommand.CreateFromObservable((FileInfo file) => SearchWithFileImpl(file));

            OpenSource = ReactiveCommand.CreateFromObservable((Uri uri) => OpenUriInteraction.Handle(uri));

            CopySource = ReactiveCommand.CreateFromObservable((Uri uri) => CopyUriInteraction.Handle(uri));

            _itemsQueue
                .Connect()
                .OnItemAdded(item => SelectedQueueItem = item)
                // Start search when items is added.
                .SubscribeMany(item => item
                    .Search
                    .Execute(SelectedSearchService!)
                    .Catch(Observable.Empty<IEnumerable<SearchResultViewModel>>())
                    .Subscribe())
                // Load the item's thumbnail.
                .SubscribeMany(item => item
                    .LoadThumbnail
                    .Execute()
                    .Subscribe())
                .Bind(out var queuedItems)
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedQueueItem)
                .WhereNotNull()
                .Select(item => item.SearchResults.ToObservableChangeSet())
                .Switch()
                // Observe commands on the selected search result and pipe their execution to the main commands.
                .SubscribeMany(result => result
                    .OpenSource
                    .Select(_ => result.SourceUri)
                    .InvokeCommand(this, x => x.OpenSource))
                .SubscribeMany(result => result
                    .CopySource
                    .Select(_ => result.SourceUri)
                    .InvokeCommand(this, x => x.CopySource))
                .SubscribeMany(result => result
                    .SearchForSimilar
                    .Select(_ => result.ImageUri)
                    .InvokeCommand(this, x => x.SearchWithUri))
                .Subscribe();

            QueuedItems = queuedItems;

            AddFile = ReactiveCommand.CreateFromObservable(
                () => SelectFileInteraction.Handle(Unit.Default).WhereNotNull());

            AddFile
                .InvokeCommand(this, x => x.SearchWithFile);

            AddUri = ReactiveCommand.Create(
                () => new Uri(ImageUri),
                this.WhenAnyValue(x => x.ImageUri, text => Uri.TryCreate(text, UriKind.Absolute, out _)));

            AddUri
                .InvokeCommand(this, x => x.SearchWithUri);

            ClearQueue = ReactiveCommand.Create(
                () => _itemsQueue.Clear(),
                _itemsQueue.CountChanged.Select(count => count > 0));
        }

        #region Properties

        [Reactive]
        public SearchServiceViewModel? SelectedSearchService { get; set; }

        [Reactive]
        public QueueItemViewModel? SelectedQueueItem { get; set; }

        public IEnumerable<SearchServiceViewModel>? SearchServices { get; } = Locator.Current.GetService<IEnumerable<SearchServiceViewModel>>();

        public ReadOnlyObservableCollection<QueueItemViewModel> QueuedItems { get; }

        [Reactive]
        public string? ImageUri { get; set; }

        #endregion

        #region Commands

        public ReactiveCommand<Uri, Unit> OpenSource { get; }
        public ReactiveCommand<Uri, Unit> CopySource { get; }
        public ReactiveCommand<Uri, Unit> SearchWithUri { get; }
        public ReactiveCommand<FileInfo, Unit> SearchWithFile { get; }
        public ReactiveCommand<Unit, FileInfo> AddFile { get; }
        public ReactiveCommand<Unit, Uri> AddUri { get; }
        public ReactiveCommand<Unit, Unit> ClearQueue { get; }

        #endregion

        #region Interactions

        public Interaction<Uri, Unit> OpenUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Uri, Unit> CopyUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Unit, FileInfo?> SelectFileInteraction { get; } = new Interaction<Unit, FileInfo?>();

        #endregion

        #region Command implementations

        private IObservable<Unit> SearchWithUriImpl(Uri uri)
        {
            var item = new UriQueueItemViewModel(uri);

            _itemsQueue.Add(item);

            return Observable.Return(Unit.Default);

        }

        private IObservable<Unit> SearchWithFileImpl(FileInfo fileInfo)
        {
            var item = new FileQueueItemViewModel(fileInfo);

            _itemsQueue.Add(item);

            return Observable.Return(Unit.Default);
        }

        #endregion
    }
}
