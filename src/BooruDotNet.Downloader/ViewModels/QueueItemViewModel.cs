using System;
using BooruDotNet.Posts;
using Easy.Common;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class QueueItemViewModel : ReactiveObject
    {
        public QueueItemViewModel(IPost post)
        {
            Ensure.NotNull(post, nameof(post));
            Post = post;
        }

        public IPost Post { get; }
    }
}
