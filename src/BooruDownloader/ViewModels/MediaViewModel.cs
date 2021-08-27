using System;
using BooruDownloader.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BooruDownloader.ViewModels
{
    public class MediaViewModel : ReactiveObject
    {
        public MediaViewModel()
        {
            this.WhenAnyValue(x => x.Uri, uri => uri?.IsAbsoluteUri is true && UriHelper.IsAnimatedMediaFile(uri))
                .ToPropertyEx(this, x => x.IsAnimatedMedia);

            this.WhenAnyValue(x => x.Uri, uri => uri?.IsAbsoluteUri is true && UriHelper.IsAnimatedMediaFile(uri) is false)
                .ToPropertyEx(this, x => x.IsStaticImage);
        }

        [Reactive]
        public Uri? Uri { get; set; }

        public bool IsAnimatedMedia { [ObservableAsProperty] get; }

        public bool IsStaticImage { [ObservableAsProperty] get; }
    }
}
