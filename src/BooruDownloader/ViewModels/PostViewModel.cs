using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class PostViewModel : ReactiveObject
    {
        private readonly IBooruTagByName _tagExtractor;
        private readonly ObservableAsPropertyHelper<IEnumerable<TagViewModel>> _tags;

        public PostViewModel(IPost post, IBooruTagByName tagExtractor)
        {
            Post = Requires.NotNull(post, nameof(post));
            _tagExtractor = Requires.NotNull(tagExtractor, nameof(tagExtractor));

            // TODO: if tag extractor is null, add all tags as regular tags. Needs separate tag class for this.
            // TODO: add something that makes it obvious that tags are loading.

            _tags = Observable.StartAsync(GetTags, RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Tags);
        }

        public IPost Post { get; }

        public IEnumerable<TagViewModel> Tags => _tags.Value;

        private async Task<IEnumerable<TagViewModel>> GetTags(CancellationToken cancellationToken)
        {
            var tags = new List<TagViewModel>(Post.Tags.Length);

            foreach (var postTag in Post.Tags)
            {
                try
                {
                    var tag = await _tagExtractor.GetTagAsync(postTag, cancellationToken);

                    tags.Add(new TagViewModel(tag));
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                    {
                        throw;
                    }
                }
            }

            return tags;
        }
    }
}
