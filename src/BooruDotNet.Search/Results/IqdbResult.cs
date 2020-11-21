using System;
using BooruDotNet.Resources;
using Easy.Common;

namespace BooruDotNet.Search.Results
{
    public class IqdbResult : IResult
    {
        public IqdbResult(Uri source, Uri previewImageUri, int width, int height, double similarity)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.That(source.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

            Ensure.NotNull(previewImageUri, nameof(previewImageUri));
            Ensure.That(previewImageUri.IsAbsoluteUri, ErrorMessages.UriIsNotAbsolute);

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
