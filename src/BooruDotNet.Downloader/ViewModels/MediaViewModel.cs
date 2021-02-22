using System;
using BooruDotNet.Downloader.Helpers;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class MediaViewModel : ReactiveObject
    {
        private Uri _uri;
        private readonly ObservableAsPropertyHelper<bool> _isAnimatedMedia;
        private readonly ObservableAsPropertyHelper<bool> _isStaticImage;

        public MediaViewModel()
        {
            _isAnimatedMedia = this.WhenAnyValue(x => x.Uri, uri => uri?.IsAbsoluteUri == true && UriHelper.IsAnimatedMediaFile(uri))
                .ToProperty(this, x => x.IsAnimatedMedia);

            _isStaticImage = this.WhenAnyValue(x => x.Uri, uri => uri?.IsAbsoluteUri == true && !UriHelper.IsAnimatedMediaFile(uri))
                .ToProperty(this, x => x.IsStaticImage);
        }

        public Uri Uri
        {
            get => _uri;
            set => this.RaiseAndSetIfChanged(ref _uri, value);
        }

        public bool IsAnimatedMedia => _isAnimatedMedia.Value;

        public bool IsStaticImage => _isStaticImage.Value;
    }
}
