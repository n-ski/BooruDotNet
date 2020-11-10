using System;
using System.Collections.Immutable;

namespace BooruDotNet.Posts
{
    public interface IPost
    {
        internal const string DebuggerDisplayString = "{Uri.AbsoluteUri,nq}";

        public int? ID { get; }
        public Uri Uri { get; }
        public DateTime CreationDate { get; }
        public int Width { get; }
        public int Height { get; }
        public long? FileSize { get; }
        public string Hash { get; }
        public Uri? FileUri { get; }
        public Uri? SampleImageUri { get; }
        public Uri? PreviewImageUri { get; }
        ImmutableArray<string> Tags { get; }
        Rating Rating { get; }
        string Source { get; }
        int? Score { get; }
    }
}
