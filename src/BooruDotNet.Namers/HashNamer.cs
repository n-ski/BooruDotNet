using BooruDotNet.Posts;
using Easy.Common;

namespace BooruDotNet.Namers
{
    public class HashNamer : IPostNamer
    {
        public string Name(IPost post)
        {
            Ensure.NotNull(post, nameof(post));

            return post.Hash;
        }
    }
}
