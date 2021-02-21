using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public MainViewModel()
        {
            _queuedItems = new ObservableCollectionExtended<QueueItemViewModel>();
            QueuedItems = new ReadOnlyObservableCollection<QueueItemViewModel>(_queuedItems);

            // TODO https://www.reactiveui.net/docs/handbook/commands/canceling#cancellation-with-the-task-parallel-library

            AddFromFile = ReactiveCommand.CreateFromTask(AddFromFileImpl);

            AddFromFile.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            RemoveSelection = ReactiveCommand.Create(
                () => _queuedItems.RemoveMany(SelectedItems),
                this.WhenAnyValue(x => x.SelectedItems, items => items?.Any() == true));

            _isAddingPosts = AddFromFile.IsExecuting.ToProperty(this, x => x.IsAddingPosts);

            DownloadPosts = ReactiveCommand.CreateFromTask(
                DownloadPostsImpl,
                this.WhenAnyValue(
                    x => x.IsAddingPosts,
                    x => x.QueuedItems.Count,
                    (isAdding, count) => !isAdding && count > 0));

            DownloadPosts.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));
        }

        public ReadOnlyObservableCollection<QueueItemViewModel> QueuedItems { get; }

        public IEnumerable<QueueItemViewModel> SelectedItems
        {
            get => _selectedItems;
            set => this.RaiseAndSetIfChanged(ref _selectedItems, value);
        }

        public bool IsAddingPosts => _isAddingPosts.Value;

        public ReactiveCommand<Unit, Unit> AddFromFile { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelection { get; }

        public ReactiveCommand<Unit, Unit> DownloadPosts { get; }

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
            foreach (string url in links)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    if (_queuedItems.Any(vm => vm.Post.Uri == uri))
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipped duplicate URL '{uri}'.", GetType().Name);
                        continue;
                    }

                    var post = await LinkResolver.ResolveAsync(uri, cancellationToken);

                    // Skip if couldn't resolve the link or if the file is private/hidden.
                    // TODO: notify about skipped posts.
                    if (post?.FileUri is null)
                    {
                        continue;
                    }

                    var item = new QueueItemViewModel(post);
                    _queuedItems.Add(item);
                }
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
            downloader.BatchSize = 4; // TODO: needs to be a setting.

            var posts = QueuedItems.Select(q => q.Post);
            long totalBytes = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            await foreach (var file in downloader.DownloadAsync(posts, directory.FullName, cancellationToken))
            {
                totalBytes += file.Length;

                System.Diagnostics.Debug.WriteLine(
                    $"Saved file '{file.FullName}' ({ByteSize.FromBytes(file.Length).Humanize("0.00")}).",
                    GetType().Name);
            };

            sw.Stop();

            var byteSize = ByteSize.FromBytes(totalBytes);
            var rate = byteSize.Per(sw.Elapsed);

            System.Diagnostics.Debug.WriteLine(
                $"Downloaded {byteSize.Humanize("0.00")} total in {sw.Elapsed.TotalSeconds:F3} s ({rate.Humanize("0.00")} avg).",
                GetType().Name);
        }
    }
}
