using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class PostViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<IEnumerable<TagViewModel>> _tags;

        public PostViewModel(IPost post, IBooruTagByName tagExtractor)
        {
            Post = Requires.NotNull(post, nameof(post));
            Requires.NotNull(tagExtractor, nameof(tagExtractor));

            _tags = Observable.Start(() => Post.Tags.Select(tag => new TagViewModel(tag, tagExtractor)))
                .ToProperty(this, x => x.Tags);
        }

        public IPost Post { get; }

        public IEnumerable<TagViewModel> Tags => _tags.Value;
    }
}
