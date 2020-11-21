using System;

namespace BooruDotNet.Search.Results
{
    public interface IResult
    {
        Uri Source { get; }
        Uri PreviewImageUri { get; }
        int Width { get; }
        int Height { get; }
        double Similarity { get; }
    }
}
