using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.WPF.Interactions;
using ReactiveUI;
using Validation;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class ResultViewModel : ReactiveObject
    {
        private readonly IResult _result;

        public ResultViewModel(IResult result)
        {
            _result = Requires.NotNull(result, nameof(result));
            ImageSize = new Size(_result.Width, _result.Height);

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
                () => ImageInteractions.SearchForSimilar.Handle(ImageUri));

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
