using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Links;
using DynamicData;
using DynamicData.Binding;
using Humanizer;
using Humanizer.Bytes;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableCollectionExtended<QueueItemViewModel> _queuedItems;
        private IEnumerable<QueueItemViewModel> _selectedItems;
        private readonly ObservableAsPropertyHelper<bool> _isAddingPosts;
        private readonly ObservableAsPropertyHelper<bool> _isDownloading;
        private readonly ObservableAsPropertyHelper<bool> _isBusy;
        private int _progressValue;
        private int _progressMaximum;

        public MainViewModel()
        {
            _queuedItems = new ObservableCollectionExtended<QueueItemViewModel>();
            QueuedItems = new ReadOnlyObservableCollection<QueueItemViewModel>(_queuedItems);

            // TODO https://www.reactiveui.net/docs/handbook/commands/canceling#cancellation-with-the-task-parallel-library

            AddFromFile = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(AddFromFileImpl).TakeUntil(CancelAdd));

            AddFromFile.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            RemoveSelection = ReactiveCommand.Create(
                () => _queuedItems.RemoveMany(SelectedItems),
                this.WhenAnyValue(x => x.SelectedItems, items => items?.Any() == true));

            _isAddingPosts = AddFromFile.IsExecuting.ToProperty(this, x => x.IsAddingPosts);

            CancelAdd = ReactiveCommand.Create(
                ReactiveHelper.DoNothing,
                this.WhenAnyValue(x => x.IsAddingPosts));

            DownloadPosts = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(DownloadPostsImpl).TakeUntil(CancelDownload),
                this.WhenAnyValue(
                    x => x.IsAddingPosts,
                    x => x.QueuedItems.Count,
                    (isAdding, count) => !isAdding && count > 0));

            _isDownloading = DownloadPosts.IsExecuting.ToProperty(this, x => x.IsDownloading);

            CancelDownload = ReactiveCommand.Create(
                ReactiveHelper.DoNothing,
                this.WhenAnyValue(x => x.IsDownloading));

            _isBusy = Observable.Merge(
                this.WhenAnyValue(x => x.IsAddingPosts),
                this.WhenAnyValue(x => x.IsDownloading))
                .ToProperty(this, x => x.IsBusy);

            DownloadPosts.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            OpenSettingsInteraction = new Interaction<Unit, Unit>();

            OpenSettingsCommand = ReactiveCommand.CreateFromObservable(
                () => OpenSettingsInteraction.Handle(Unit.Default));
        }

        public ReadOnlyObservableCollection<QueueItemViewModel> QueuedItems { get; }

        public IEnumerable<QueueItemViewModel> SelectedItems
        {
            get => _selectedItems;
            set => this.RaiseAndSetIfChanged(ref _selectedItems, value);
        }

        public int ProgressValue
        {
            get => _progressValue;
            private set => this.RaiseAndSetIfChanged(ref _progressValue, value);
        }

        public int ProgressMaximum
        {
            get => _progressMaximum;
            private set => this.RaiseAndSetIfChanged(ref _progressMaximum, value);
        }

        public bool IsAddingPosts => _isAddingPosts.Value;

        public bool IsDownloading => _isDownloading.Value;

        public bool IsBusy => _isBusy.Value;

        public Interaction<Unit, Unit> OpenSettingsInteraction { get; }

        public ReactiveCommand<Unit, Unit> AddFromFile { get; }

        public ReactiveCommand<Unit, Unit> CancelAdd { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelection { get; }

        public ReactiveCommand<Unit, Unit> DownloadPosts { get; }

        public ReactiveCommand<Unit, Unit> CancelDownload { get; }

        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

        private async Task AddFromFileImpl(CancellationToken cancellationToken)
        {
            var file = await Interactions.OpenFileBrowser.Handle("Text files|*.txt");

            // Dialog was closed.
            if (file is null)
            {
                return;
            }

            var links = await File.ReadAllLinesAsync(file.FullName, cancellationToken);

            await ResolveLinksAsync(links, cancellationToken);
        }

        private async Task ResolveLinksAsync(IEnumerable<string> links, CancellationToken cancellationToken)
        {
            var urisToResolve = new List<Uri>(links.Count());

            foreach (string url in links)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    if (_queuedItems.Any(vm => vm.Post.Uri == uri))
                    {
                        Logger.Debug($"Skipped duplicate URL '{uri}'.", this);
                        continue;
                    }

                    urisToResolve.Add(uri);
                }
            }

            ProgressValue = 0;
            ProgressMaximum = urisToResolve.Count;

            foreach (Uri uri in urisToResolve)
            {
                var post = await LinkResolver.ResolveAsync(uri, cancellationToken);

                // Skip if couldn't resolve the link or if the file is private/hidden.
                // TODO: notify about skipped posts.
                if (post?.FileUri is null)
                {
                    continue;
                }

                var item = new QueueItemViewModel(post);
                _queuedItems.Add(item);

                ++ProgressValue;
            }
        }

        private async Task DownloadPostsImpl(CancellationToken cancellationToken)
        {
            var directory = await Interactions.OpenFolderBrowser.Handle(Unit.Default);

            if (directory is null)
            {
                return;
            }

            var downloader = App.PostDownloader;
            downloader.BatchSize = Settings.Default.BatchSize;

            ProgressValue = 0;
            ProgressMaximum = QueuedItems.Count;

            var posts = QueuedItems.Select(q => q.Post);
            long totalBytes = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            await foreach (var file in downloader.DownloadAsync(posts, directory.FullName, cancellationToken))
            {
                ++ProgressValue;
                totalBytes += file.Length;

                Logger.Debug(
                    $"Saved file '{file.FullName}' ({ByteSize.FromBytes(file.Length).Humanize("0.00")}).",
                    this);
            };

            sw.Stop();

            var byteSize = ByteSize.FromBytes(totalBytes);
            var rate = byteSize.Per(sw.Elapsed);

            Logger.Debug(
                $"Downloaded {byteSize.Humanize("0.00")} total in {sw.Elapsed.TotalSeconds:F3} s ({rate.Humanize("0.00")} avg).",
                this);
        }
    }
}
