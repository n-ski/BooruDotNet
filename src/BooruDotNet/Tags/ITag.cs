namespace BooruDotNet.Tags
{
    public interface ITag
    {
        string Name { get; }
        TagKind Kind { get; }
        int ID { get; }
        int Count { get; }
    }
}
