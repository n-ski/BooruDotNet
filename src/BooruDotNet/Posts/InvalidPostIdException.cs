using System;
using System.Net.Http;
using BooruDotNet.Resources;

namespace BooruDotNet.Posts
{
    public class InvalidPostIdException : HttpRequestException
    {
        public InvalidPostIdException(int postId)
            : base(string.Format(ErrorMessages.PostInvalidIdFormat, postId))
        {
            PostID = postId;
        }

        public InvalidPostIdException(int postId, string message)
            : base(message)
        {
            PostID = postId;
        }

        public InvalidPostIdException(int postId, string message, Exception inner)
            : base(message, inner)
        {
            PostID = postId;
        }

        public int PostID { get; }
    }
}
