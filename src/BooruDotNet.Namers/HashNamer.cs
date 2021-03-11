using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Namers
{
    public class HashNamer : IPostNamer
    {
        public string Name(IPost post)
        {
            Requires.NotNull(post, nameof(post));

            return post.Hash;
        }
    }
}
