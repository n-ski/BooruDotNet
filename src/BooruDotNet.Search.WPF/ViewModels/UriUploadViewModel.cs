using System;
using System.Reactive;
using BooruDotNet.Search.WPF.Helpers;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class UriUploadViewModel : ReactiveObject
    {
        private Uri _imageUri;

        public UriUploadViewModel()
        {
            SearchCommand = ReactiveCommand.Create(CommandHelper.DoNothing);
        }

        public Uri ImageUri
        {
            get => _imageUri;
            set => this.RaiseAndSetIfChanged(ref _imageUri, value);
        }

        public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    }
}
