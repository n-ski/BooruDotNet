using BooruDotNet.Posts;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class QueueItemViewModel : ReactiveObject
    {
        public QueueItemViewModel(IPost post)
        {
            Post = Requires.NotNull(post, nameof(post));
        }

        public IPost Post { get; }
    }
}
