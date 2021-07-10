using System;
using System.Drawing;
using System.Reactive;
using BooruDotNet.Search.Results;
using ReactiveUI;
using Validation;

namespace ImageSearch.ViewModels
{
    public class SearchResultViewModel : ReactiveObject
    {
        private readonly IResult _result;

        public SearchResultViewModel(IResult result)
        {
            _result = Requires.NotNull(result, nameof(result));

            ImageSize = result.Width.HasValue && result.Height.HasValue
                ? new Size(result.Width.Value, result.Height.Value) : Size.Empty;

            OpenSource = ReactiveCommand.Create(() => SourceUri);
            CopySource = ReactiveCommand.Create(() => SourceUri);
            SearchForSimilar = ReactiveCommand.Create(() => ImageUri);
        }

        public Uri ImageUri => _result.PreviewImageUri;
        public double Similarity => _result.Similarity;
        public Uri SourceUri => _result.Source;
        public Size ImageSize { get; }

        public ReactiveCommand<Unit, Uri> OpenSource { get; }
        public ReactiveCommand<Unit, Uri> CopySource { get; }
        public ReactiveCommand<Unit, Uri> SearchForSimilar { get; }
    }
}
