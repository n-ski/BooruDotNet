using System;
using System.Drawing;
using System.Reactive;
using BooruDotNet.Search.Results;
using ImageSearch.Helpers;
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

            OpenSource = ReactiveCommand.Create(MethodHelper.DoNothing);
            CopySource = ReactiveCommand.Create(MethodHelper.DoNothing);
            SearchForSimilar = ReactiveCommand.Create(MethodHelper.DoNothing);
        }

        public Uri ImageUri => _result.PreviewImageUri;
        public double Similarity => _result.Similarity;
        public Uri SourceUri => _result.Source;
        public Size ImageSize { get; }

        public ReactiveCommand<Unit, Unit> OpenSource { get; }
        public ReactiveCommand<Unit, Unit> CopySource { get; }
        public ReactiveCommand<Unit, Unit> SearchForSimilar { get; }
    }
}
