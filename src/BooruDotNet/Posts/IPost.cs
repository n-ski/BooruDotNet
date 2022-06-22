using System;
using System.Collections.Generic;

namespace BooruDotNet.Posts;

public interface IPost
{
    internal const string DebuggerDisplayString = "{Uri.AbsoluteUri,nq}";

    public int? ID { get; }
    public Uri? Uri { get; }
    public DateTimeOffset CreationDate { get; }
    public int Width { get; }
    public int Height { get; }
    public long? FileSize { get; }
    public string Hash { get; }
    public Uri? FileUri { get; }
    public Uri? SampleImageUri { get; }
    public Uri? PreviewImageUri { get; }
    IReadOnlyList<string> Tags { get; }
    Rating Rating { get; }
    string Source { get; }
    int? Score { get; }
}
