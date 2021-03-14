using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using BooruDotNet.Search.Results;
using ImageSearch.Interactions;
using ReactiveUI;
using Validation;

namespace ImageSearch.ViewModels
{
    public class ResultViewModel : ReactiveObject
    {
        private readonly IResult _result;

        public ResultViewModel(IResult result)
        {
            _result = Requires.NotNull(result, nameof(result));
            ImageSize = _result.Width.HasValue && _result.Height.HasValue
                ? new Size(_result.Width.Value, _result.Height.Value)
                : Size.Empty;

            OpenSourceCommand = ReactiveCommand.Create(() =>
            {
                Process.Start(new ProcessStartInfo(_result.Source.ToString())
                {
                    UseShellExecute = true
                });
            });

            CopySourceUriCommand = ReactiveCommand.Create(() =>
            {
                Clipboard.SetText(_result.Source.ToString());
            });

            SearchForSimilarCommand = ReactiveCommand.CreateFromObservable(
                () => ImageInteractions.SearchWithUri.Handle(ImageUri));

            // Swallow the exception. MainViewModel handles the interaction by executing
            // SearchCommand which also handles exceptions, so interacting with an exception
            // here will in fact display 2 message boxes in case of an error.
            SearchForSimilarCommand.ThrownExceptions.Subscribe();
        }

        public Uri ImageUri => _result.PreviewImageUri;
        public double Similarity => _result.Similarity;
        public Uri SourceUri => _result.Source;
        public Size ImageSize { get; }

        public ReactiveCommand<Unit, Unit> OpenSourceCommand { get; }
        public ReactiveCommand<Unit, Unit> CopySourceUriCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchForSimilarCommand { get; }
    }
}
