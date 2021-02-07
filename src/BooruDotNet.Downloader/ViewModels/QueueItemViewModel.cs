using System;
using BooruDotNet.Posts;
using Easy.Common;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class QueueItemViewModel : ReactiveObject
    {
        private readonly IPost _post;

        public QueueItemViewModel(IPost post)
        {
            Ensure.NotNull(post, nameof(post));
            _post = post;
        }

        public Uri SourceUri => _post.Uri;
        public long? FileSize => _post.FileSize;
        public Uri PreviewImageUri => _post.PreviewImageUri;
    }
}
