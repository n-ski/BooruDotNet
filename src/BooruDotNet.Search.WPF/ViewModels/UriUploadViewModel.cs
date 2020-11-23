using System;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class UriUploadViewModel : ReactiveObject
    {
        private Uri _imageUri;

        public UriUploadViewModel()
        {
        }

        public Uri ImageUri
        {
            get => _imageUri;
            set => this.RaiseAndSetIfChanged(ref _imageUri, value);
        }
    }
}
