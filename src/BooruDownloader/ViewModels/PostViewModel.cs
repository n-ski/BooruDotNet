using BooruDotNet.Posts;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    // This VM is almost the same as the QueueItemVM, but this is required for Splat,
    // since there can be only one view for each viewmodel.
    public class PostViewModel : ReactiveObject
    {
        public PostViewModel(IPost post)
        {
            Post = Requires.NotNull(post, nameof(post));
        }

        public IPost Post { get; }
    }
}
