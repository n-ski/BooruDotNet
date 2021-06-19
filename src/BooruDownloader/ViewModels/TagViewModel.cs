﻿using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Tags;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class TagViewModel : ReactiveObject
    {
        private readonly string? _tagName;
        private readonly ObservableAsPropertyHelper<ITag> _tag;
        private readonly IBooruTagByName? _tagExtractor;

        public TagViewModel(string tagName, IBooruTagByName tagExtractor)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            _tagName = tagName;
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));

            _tag = Observable.StartAsync(GetTagInfo, RxApp.TaskpoolScheduler)
                .Where(tag => tag is object)
                .Select(tag => tag!)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Tag);
        }

        public TagViewModel(ITag tag)
        {
            Requires.NotNull(tag, nameof(tag));

            _tag = Observable.Return(tag)
                .ToProperty(this, x => x.Tag);
        }

        public string Name => _tagName ?? Tag.Name;

        public ITag Tag => _tag.Value!;

        private async Task<ITag?> GetTagInfo(CancellationToken cancellationToken)
        {
            Debug.Assert(_tagExtractor is object);

            try
            {
                return await _tagExtractor.GetTagAsync(Name, cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
