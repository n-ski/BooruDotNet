using System.Collections.Immutable;

namespace BooruDotNet.Posts
{
    public interface IPostExtendedTags : IPost
    {
        ImmutableArray<string> ArtistTags { get; }
        ImmutableArray<string> CharacterTags { get; }
        ImmutableArray<string> CopyrightTags { get; }
        ImmutableArray<string> GeneralTags { get; }
        ImmutableArray<string> MetaTags { get; }
    }
}