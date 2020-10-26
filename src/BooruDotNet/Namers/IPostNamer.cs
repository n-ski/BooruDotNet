using BooruDotNet.Posts;

namespace BooruDotNet.Namers
{
    public interface IPostNamer
    {
        string Name(IPost post);
    }
}
