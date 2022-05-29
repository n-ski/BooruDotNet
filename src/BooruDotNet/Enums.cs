namespace BooruDotNet
{
    public enum Rating
    {
        Safe,
        Questionable,
        Explicit,
        General,
        Sensitive,
    }

    public enum TagKind
    {
        General = 0,
        Artist = 1,
        Copyright = 3,
        Character = 4,
        Metadata = 5,
    }
}
