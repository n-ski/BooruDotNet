using System;
using System.Collections.Immutable;

namespace BooruDotNet.Posts
{
    public interface IPost
    {
        public int ID { get; }
        public Uri Uri { get; }
        public string Hash { get; }
        public Uri FileUri { get; }
        public Uri? SampleImageUri { get; }
        public Uri? PreviewImageUri { get; }
        ImmutableArray<string> Tags { get; }
    }
}
