namespace BooruDotNet.Tags
{
    public interface ITag
    {
        internal const string DebuggerDisplayString = "{Name,nq} ({Kind}, {Count} occurrences)";

        string Name { get; }
        TagKind Kind { get; }
        int ID { get; }
        int Count { get; }
    }
}
