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
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly ObservableCollection<QueueItemViewModel> _queuedItems;
        private IEnumerable<QueueItemViewModel> _selectedItems;
        private readonly ObservableAsPropertyHelper<bool> _isAddingPosts;

        public MainViewModel()
        {
            _queuedItems = new ObservableCollection<QueueItemViewModel>();
            QueuedItems = new ReadOnlyObservableCollection<QueueItemViewModel>(_queuedItems);

            // TODO https://www.reactiveui.net/docs/handbook/commands/canceling#cancellation-with-the-task-parallel-library

            AddFromFile = ReactiveCommand.CreateFromTask(AddFromFileImpl);

            AddFromFile.ThrownExceptions.Subscribe(
                async ex => await Interactions.ShowErrorMessage.Handle(ex));

            RemoveSelection = ReactiveCommand.Create(
                () => _queuedItems.RemoveMany(SelectedItems),
                this.WhenAnyValue(x => x.SelectedItems, items => items?.Any() == true));

            _isAddingPosts = AddFromFile.IsExecuting.ToProperty(this, x => x.IsAddingPosts);
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
    }
}
