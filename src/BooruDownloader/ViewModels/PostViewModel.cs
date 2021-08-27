using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using BooruDotNet.Posts;
using BooruDotNet.Tags;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class PostViewModel : ReactiveObject
    {
        public PostViewModel(IPost post, IBooruTagByName tagExtractor)
            : this(post)
        {
            Requires.NotNull(tagExtractor, nameof(tagExtractor));

            Observable.Return(Post.Tags)
                .Select(tags => tags.Select(tag => new TagViewModel(tag, tagExtractor)))
                .ToPropertyEx(this, x => x.Tags);
        }

        public PostViewModel(IPostExtendedTags post)
            : this((IPost)post)
        {
            Observable.Return(((IPostExtendedTags)Post).ExtendedTags)
                .Select(tags => tags.Select(tag => new TagViewModel(tag)))
                .ToPropertyEx(this, x => x.Tags);
        }

        private PostViewModel(IPost post)
        {
            Post = Requires.NotNull(post, nameof(post));
            Requires.Argument(Post.Uri is object, nameof(post), "Uri must not be null.");

            OpenInBrowser = ReactiveCommand.Create(() =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Post.Uri!.AbsoluteUri,
                    UseShellExecute = true,
                };

                using (Process.Start(startInfo))
                {
                }
            });
        }

        public IPost Post { get; }

        public extern IEnumerable<TagViewModel> Tags { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, Unit> OpenInBrowser { get; }
    }
}
