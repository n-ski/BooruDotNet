using BooruDotNet.Posts;

namespace BooruDotNet.Search.Results
{
    public interface IResult
    {
        double Similarity { get; }
        IPost Post { get; }
    }
}
