using BooruDotNet.Posts;
using Easy.Common;
using ReactiveUI;

namespace BooruDownloader.ViewModels
{
    // This VM is almost the same as the QueueItemVM, but this is required for Splat,
    // since there can be only one view for each viewmodel.
    public class PostViewModel : ReactiveObject
    {
        public PostViewModel(IPost post)
        {
            Post = Ensure.NotNull(post, nameof(post));
        }

        public IPost Post { get; }
    }
}
