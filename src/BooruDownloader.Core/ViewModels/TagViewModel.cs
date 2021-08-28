using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Tags;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class TagViewModel : ReactiveObject
    {
        private readonly string? _tagName;
        private readonly IBooruTagByName? _tagExtractor;

        public TagViewModel(string tagName, IBooruTagByName tagExtractor)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            _tagName = tagName;
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));

            Observable.StartAsync(GetTagInfo, RxApp.TaskpoolScheduler)
                .WhereNotNull()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.Tag);
        }

        public TagViewModel(ITag tag)
        {
            Requires.NotNull(tag, nameof(tag));

            Observable.Return(tag).ToPropertyEx(this, x => x.Tag);
        }

        public string Name => _tagName ?? Tag!.Name;

        public extern ITag? Tag { [ObservableAsProperty] get; }

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
