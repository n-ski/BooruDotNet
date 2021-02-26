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
        private readonly ObservableAsPropertyHelper<ITag> _tag;
        private readonly IBooruTagByName _tagExtractor;

        public TagViewModel(string tagName, IBooruTagByName tagExtractor)
        {
            Requires.NotNullOrWhiteSpace(tagName, nameof(tagName));

            Name = tagName;
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));

            _tag = Observable.StartAsync(GetTagInfo, RxApp.TaskpoolScheduler)
                .Where(tag => tag != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Tag);
        }

        public string Name { get; }

        public ITag Tag => _tag.Value;

        private async Task<ITag> GetTagInfo(CancellationToken cancellationToken)
        {
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
