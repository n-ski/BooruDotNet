using System;
using BooruDotNet.Extensions;
using Validation;

namespace BooruDotNet.Search.Results
{
    internal sealed class IqdbResult : IResult
    {
        public IqdbResult(Uri source, Uri previewImageUri, int? width, int? height, double similarity)
        {
            Requires.NotNull(source, nameof(source));
            source.RequireAbsolute(nameof(source));

            Requires.NotNull(previewImageUri, nameof(previewImageUri));
            previewImageUri.RequireAbsolute(nameof(previewImageUri));

            Source = source;
            PreviewImageUri = previewImageUri;
            Width = width;
            Height = height;
            Similarity = similarity;
        }

        public Uri Source { get; }

        public Uri PreviewImageUri { get; }

        public int? Width { get; }

        public int? Height { get; }

        public double Similarity { get; }
    }
}
