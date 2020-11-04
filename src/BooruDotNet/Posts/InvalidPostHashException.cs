using System;
using System.Net.Http;
using BooruDotNet.Resources;

namespace BooruDotNet.Posts
{
    public class InvalidPostHashException : HttpRequestException
    {
        public InvalidPostHashException(string hash)
            : base(string.Format(ErrorMessages.PostInvalidHashFormat, hash))
        {
            Hash = hash;
        }

        public InvalidPostHashException(string hash, string message)
            : base(message)
        {
            Hash = hash;
        }

        public InvalidPostHashException(string hash, string message, Exception inner)
            : base(message, inner)
        {
            Hash = hash;
        }

        public string Hash { get; }
    }
}
