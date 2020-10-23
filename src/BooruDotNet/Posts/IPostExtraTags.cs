using System.Collections.Generic;

namespace BooruDotNet.Posts
{
    public interface IPostExtraTags : IPost
    {
        IReadOnlyList<string> ArtistTags { get; }
        IReadOnlyList<string> CharacterTags { get; }
        IReadOnlyList<string> CopyrightTags { get; }
        IReadOnlyList<string> GeneralTags { get; }
        IReadOnlyList<string> MetaTags { get; }
    }
}