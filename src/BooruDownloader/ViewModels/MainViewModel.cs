﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet;
using BooruDotNet.Downloaders;
using BooruDotNet.Links;
using BooruDotNet.Posts;
using BooruDownloader.Helpers;
using DynamicData;
using DynamicData.Binding;
using Humanizer;
using Humanizer.Bytes;
using ReactiveUI;

namespace BooruDownloader.ViewModels
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

            AddFromUrls = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(AddFromUrlsImpl).TakeUntil(CancelAdd));

            AddFromUrls.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            AddFromFile = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(AddFromFileImpl).TakeUntil(CancelAdd));

            AddFromFile.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            RemoveSelection = ReactiveCommand.Create(
                () => _queuedItems.RemoveMany(SelectedItems),
                this.WhenAnyValue(x => x.SelectedItems, items => items?.Any() == true));

            ClearQueue = ReactiveCommand.Create(
                _queuedItems.Clear,
                this.WhenAnyValue(x => x._queuedItems.Count, count => count > 0));

            _isAddingPosts = Observable.Merge(AddFromUrls.IsExecuting, AddFromFile.IsExecuting)
                .ToProperty(this, x => x.IsAddingPosts);

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

            OpenUrlInputDialog = new Interaction<Unit, IEnumerable<string>>();

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

        public Interaction<Unit, IEnumerable<string>> OpenUrlInputDialog { get; }

        public Interaction<Unit, Unit> OpenSettingsInteraction { get; }

        public ReactiveCommand<Unit, Unit> AddFromUrls { get; }

        public ReactiveCommand<Unit, Unit> AddFromFile { get; }

        public ReactiveCommand<Unit, Unit> CancelAdd { get; }

        public ReactiveCommand<Unit, Unit> RemoveSelection { get; }

        public ReactiveCommand<Unit, Unit> ClearQueue { get; }

        public ReactiveCommand<Unit, Unit> DownloadPosts { get; }

        public ReactiveCommand<Unit, Unit> CancelDownload { get; }

        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

        private async Task AddFromUrlsImpl(CancellationToken cancellationToken)
        {
            var links = await OpenUrlInputDialog.Handle(Unit.Default);

            // Dialog was closed or no links specified.
            if (links?.Any() != true)
            {
                return;
            }

            await ResolveLinksAsync(links, cancellationToken);
        }

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
            int duplicateUrlCount = 0;

            foreach (string url in links)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    if (_queuedItems.Any(vm => vm.Post.Uri == uri))
                    {
                        ++duplicateUrlCount;
                        Logger.Debug($"Skipped duplicate URL '{uri}'.", this);
                        continue;
                    }

                    urisToResolve.Add(uri);
                }
            }

            ProgressValue = 0;
            ProgressMaximum = urisToResolve.Count;
            int unresolvedPostCount = 0;

            foreach (Uri uri in urisToResolve)
            {
                IPost post;

                try
                {
                    post = await LinkResolver.ResolveAsync(uri, cancellationToken);
                }
                catch (ApiException)
                {
                    ++unresolvedPostCount;
                    continue;
                }

                // Skip if couldn't resolve the link or if the file is private/hidden.
                if (post?.FileUri is null)
                {
                    ++unresolvedPostCount;
                    continue;
                }

                var item = new QueueItemViewModel(post);
                _queuedItems.Add(item);

                ++ProgressValue;
            }

            if (Settings.Default.NotifyAboutSkippedPosts)
            {
                bool hasDuplicates = duplicateUrlCount > 0;
                bool hasUnresolvedPosts = unresolvedPostCount > 0;

                if (hasDuplicates || hasUnresolvedPosts)
                {
                    int errorsTotal = duplicateUrlCount + unresolvedPostCount;

                    var messageBuilder = new StringBuilder().AppendFormat(
                        "{0} {1} not added.",
                        "post".ToQuantity(errorsTotal),
                        errorsTotal > 1 ? "were" : "was");

                    if (hasDuplicates)
                    {
                        // TODO: needs a better message.
                        messageBuilder
                            .Append(Environment.NewLine)
                            .AppendFormat(
                                "> {0} {1} already in the queue.",
                                "post".ToQuantity(duplicateUrlCount),
                                duplicateUrlCount > 1 ? "are" : "is");
                    }

                    if (hasUnresolvedPosts)
                    {
                        messageBuilder
                            .Append(Environment.NewLine)
                            .AppendFormat(
                                "> {0} {1} skipped (invalid input URL or file URL is missing).",
                                "post".ToQuantity(unresolvedPostCount),
                                unresolvedPostCount > 1 ? "were" : "was");
                    }

                    await Interactions.ShowWarning.Handle(messageBuilder.ToString());
                }
            }
        }

        private async Task DownloadPostsImpl(CancellationToken cancellationToken)
        {
            var settings = Settings.Default;
            string targetDirectory;

            static async Task<string> showFolderDialog()
            {
                var directory = await Interactions.OpenFolderBrowser.Handle(Unit.Default);

                return directory?.FullName;
            }

            if (settings.AskLocationBeforeDownload)
            {
                targetDirectory = await showFolderDialog();
            }
            else if (!Directory.Exists(settings.DownloadLocation))
            {
                // Let the user know that directory doesn't exist anymore.
                await Interactions.ShowWarning.Handle(string.Join(
                    Environment.NewLine,
                    $"Directory '{settings.DownloadLocation}' doesn't exist.",
                    "Download directory setting was changed to the default."));

                targetDirectory = await showFolderDialog();
            }
            else
            {
                targetDirectory = settings.DownloadLocation;
            }

            if (targetDirectory is null)
            {
                return;
            }

            var batchSize = settings.BatchSize;
            var options = new PostDownloaderOptions
            {
                BatchSize = batchSize,
                OverwriteExisting = settings.OverwriteExistingFiles,
                IgnoreArchiveFiles = settings.IgnoreArchiveFiles,
            };

            var downloader = App.Downloaders[settings.FileNamingStyle];
            downloader.Options = options;

            Logger.Debug(
                $"Begin download ({"file".ToQuantity(QueuedItems.Count)}, {"thread".ToQuantity(batchSize)}, {downloader.GetType().Name}, {settings.FileNamingStyle}).",
                this);

            ProgressValue = 0;
            ProgressMaximum = QueuedItems.Count;

            var posts = QueuedItems.Select(q => q.Post);
            long totalBytes = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            await foreach (var file in downloader.DownloadAsync(posts, targetDirectory, cancellationToken))
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

            if (settings.PlaySoundWhenComplete)
            {
                SystemSounds.Asterisk.Play();
            }
        }
    }
}