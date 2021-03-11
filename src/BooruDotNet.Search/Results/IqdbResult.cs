using System;
using BooruDotNet.Resources;
using Validation;

namespace BooruDotNet.Search.Results
{
    public class IqdbResult : IResult
    {
        public IqdbResult(Uri source, Uri previewImageUri, int width, int height, double similarity)
        {
            Requires.NotNull(source, nameof(source));
            Requires.Argument(source.IsAbsoluteUri, nameof(source), ErrorMessages.UriIsNotAbsolute);

            Requires.NotNull(previewImageUri, nameof(previewImageUri));
            Requires.Argument(source.IsAbsoluteUri, nameof(previewImageUri), ErrorMessages.UriIsNotAbsolute);

            Source = source;
            PreviewImageUri = previewImageUri;
            Width = width;
            Height = height;
            Similarity = similarity;
        }

        public Uri Source { get; }

        public Uri PreviewImageUri { get; }

        public int Width { get; }

        public int Height { get; }

        public double Similarity { get; }
    }
}
