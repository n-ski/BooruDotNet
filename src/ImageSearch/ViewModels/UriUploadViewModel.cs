using System;
using System.Reactive;
using ImageSearch.Interactions;
using ReactiveUI;

namespace ImageSearch.ViewModels
{
    public class UriUploadViewModel : ReactiveObject
    {
        private Uri _imageUri;

        public UriUploadViewModel()
        {
            SearchCommand = ReactiveCommand.CreateFromObservable(
                () => ImageInteractions.SearchWithUri.Handle(ImageUri));

            // Swallow the exception. MainViewModel handles the interaction by executing
            // SearchCommand which also handles exceptions, so interacting with an exception
            // here will in fact display 2 message boxes in case of an error.
            SearchCommand.ThrownExceptions.Subscribe();
        }

        public Uri ImageUri
        {
            get => _imageUri;
            set => this.RaiseAndSetIfChanged(ref _imageUri, value);
        }

        public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    }
}
