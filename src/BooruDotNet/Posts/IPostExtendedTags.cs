using System.Collections.Generic;
using BooruDotNet.Tags;

namespace BooruDotNet.Posts
{
    public interface IPostExtendedTags : IPost
    {
        IReadOnlyList<ITag> ExtendedTags { get; }
    }
}
