using BooruDotNet.Posts;

namespace BooruDotNet.Search.Results
{
    public interface IResultWithPost : IResult
    {
        IPost Post { get; set; }
    }
}