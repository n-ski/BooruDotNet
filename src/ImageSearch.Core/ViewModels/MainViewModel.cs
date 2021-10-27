using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Validation;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly HttpClient _httpClient;
        private readonly SourceList<QueueItemViewModel> _itemsQueue;
        private static readonly TimeSpan _delayBetweenMultipleSearches = TimeSpan.FromMilliseconds(100);

        public MainViewModel(HttpClient httpClient)
        {
            _httpClient = Requires.NotNull(httpClient, nameof(httpClient));
            _itemsQueue = new SourceList<QueueItemViewModel>();

            SearchServices = ImageSearch.SearchServices.Initialize(_httpClient);
            SelectedSearchService = SearchServices.First();

            SearchWithUri = ReactiveCommand.CreateFromObservable((Uri uri) => SearchWithUriImpl(uri));

            SearchWithFile = ReactiveCommand.CreateFromObservable((FileInfo file) => SearchWithFileImpl(file));

            SearchWithManyFiles = ReactiveCommand.CreateFromTask((IEnumerable<FileInfo> files) => SearchWithManyFilesImpl(files));

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
                // Search again when the retry button is pressed.
                .SubscribeMany(item => item.StatusViewModel
                    .Retry
                    .Select(_ => SelectedSearchService)
                    .Cast<IFileAndUriSearchService>()
                    .InvokeCommand(item, i => i.Search))
                // Remove the item.
                .SubscribeMany(item => item.StatusViewModel
                    .Remove
                    .Subscribe(_ => _itemsQueue.Remove(item)))
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

            AddFiles = ReactiveCommand.CreateFromObservable(
                () => SelectFilesInteraction.Handle(Unit.Default).WhereNotNull());

            AddFiles
                .InvokeCommand(this, x => x.SearchWithManyFiles);

            AddUri = ReactiveCommand.Create(
                () => new Uri(ImageUri),
                this.WhenAnyValue(x => x.ImageUri, text => Uri.TryCreate(text, UriKind.Absolute, out _)));

            AddUri
                .InvokeCommand(this, x => x.SearchWithUri);

            ClearQueue = ReactiveCommand.Create(
                () => _itemsQueue.Clear(),
                _itemsQueue.CountChanged.Select(count => count > 0));

            OpenSettings = ReactiveCommand.CreateFromTask(OpenSettingsImpl);
        }

        #region Properties

        [Reactive]
        public SearchServiceViewModel? SelectedSearchService { get; set; }

        [Reactive]
        public QueueItemViewModel? SelectedQueueItem { get; set; }

        public IEnumerable<SearchServiceViewModel> SearchServices { get; }

        public ReadOnlyObservableCollection<QueueItemViewModel> QueuedItems { get; }

        [Reactive]
        public string? ImageUri { get; set; }

        #endregion

        #region Commands

        public ReactiveCommand<Uri, Unit> OpenSource { get; }
        public ReactiveCommand<Uri, Unit> CopySource { get; }
        public ReactiveCommand<Uri, Unit> SearchWithUri { get; }
        public ReactiveCommand<FileInfo, Unit> SearchWithFile { get; }
        public ReactiveCommand<IEnumerable<FileInfo>, Unit> SearchWithManyFiles { get; }
        public ReactiveCommand<Unit, IEnumerable<FileInfo>> AddFiles { get; }
        public ReactiveCommand<Unit, Uri> AddUri { get; }
        public ReactiveCommand<Unit, Unit> ClearQueue { get; }
        public ReactiveCommand<Unit, Unit> OpenSettings { get; }

        #endregion

        #region Interactions

        public Interaction<Uri, Unit> OpenUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Uri, Unit> CopyUriInteraction { get; } = new Interaction<Uri, Unit>();
        public Interaction<Unit, IEnumerable<FileInfo>?> SelectFilesInteraction { get; } = new Interaction<Unit, IEnumerable<FileInfo>?>();
        public Interaction<SettingsViewModel, bool> ShowSettingsDialog { get; } = new Interaction<SettingsViewModel, bool>();

        #endregion

        #region Command implementations

        private IObservable<Unit> SearchWithUriImpl(Uri uri)
        {
            var item = new UriQueueItemViewModel(uri, _httpClient);

            _itemsQueue.Add(item);

            return Observable.Return(Unit.Default);

        }

        private IObservable<Unit> SearchWithFileImpl(FileInfo fileInfo)
        {
            var item = new FileQueueItemViewModel(fileInfo);

            _itemsQueue.Add(item);

            return Observable.Return(Unit.Default);
        }

        private async Task SearchWithManyFilesImpl(IEnumerable<FileInfo> files)
        {
            foreach (FileInfo fileInfo in files)
            {
                await SearchWithFile.Execute(fileInfo);

                await Task.Delay(_delayBetweenMultipleSearches);
            }
        }

        private async Task OpenSettingsImpl()
        {
            var settings = ApplicationSettings.Default;
            var settingsViewModel = new SettingsViewModel(settings);

            bool result = await ShowSettingsDialog.Handle(settingsViewModel);

            if (result is false)
            {
                return;
            }

            settings.EnableFiltering = settingsViewModel.EnableFiltering;
            settings.MinSimilarity = settingsViewModel.MinSimilarity;
            settings.EnableImageCompression = settingsViewModel.EnableImageCompression;

            settings.Save();
        }

        #endregion
    }
}
