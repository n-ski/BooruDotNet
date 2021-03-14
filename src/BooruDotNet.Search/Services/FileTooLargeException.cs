using System;
using System.Runtime.Serialization;
using BooruDotNet.Search.Resources;

namespace BooruDotNet.Search.Services
{
    [Serializable]
    public class FileTooLargeException : Exception
    {
        private readonly string _message;

        public FileTooLargeException()
            : this(null, null, null, null, null)
        {
        }

        public FileTooLargeException(string? message)
            : this(message, null, null, null, null)
        {
        }

        public FileTooLargeException(string? message, Exception? innerException)
            : this(message, null, null, null, innerException)
        {
        }

        public FileTooLargeException(string? fileName, long? fileSize, long? maxFileSize)
            : this(null, fileName, fileSize, maxFileSize, null)
        {
        }

        public FileTooLargeException(string? fileName, long? fileSize, long? maxFileSize, Exception? innerException)
            : this(null, fileName, fileSize, maxFileSize, innerException)
        {
        }

        public FileTooLargeException(string? message, string? fileName, long? fileSize, long? maxFileSize)
            : this(message, fileName, fileSize, maxFileSize, null)
        {
        }

        public FileTooLargeException(string? message, string? fileName, long? fileSize, long? maxFileSize, Exception? innerException)
            : base(message, innerException)
        {
            FileName = fileName;
            FileSize = fileSize;
            MaximumFileSize = maxFileSize;

            _message = GetMessageString(message);
        }

        protected FileTooLargeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _message = base.Message;
        }

        public string? FileName { get; }

        public long? FileSize { get; }

        public long? MaximumFileSize { get; }

        public override string Message => _message;

        private string GetMessageString(string? message)
        {
            if (message is null)
            {
                if (FileName is null)
                {
                    return ErrorMessages.FileTooLarge;
                }

                return string.Format(ErrorMessages.FileTooLarge_Format, FileName);
            }

            return base.Message;
        }
    }
}
