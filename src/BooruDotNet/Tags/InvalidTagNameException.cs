using System;
using BooruDotNet.Resources;

namespace BooruDotNet.Tags;

[Serializable]
public class InvalidTagNameException : ApiException
{
    public InvalidTagNameException(string tagName)
        : base(string.Format(ErrorMessages.TagInvalidNameFormat, tagName))
    {
        TagName = tagName;
    }

    public InvalidTagNameException(string tagName, string message)
        : base(message)
    {
        TagName = tagName;
    }

    public InvalidTagNameException(string tagName, string message, Exception inner)
        : base(message, inner)
    {
        TagName = tagName;
    }

    public string TagName { get; }
}
