using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easy.Common;
using Easy.Common.Extensions;

namespace BooruDownloader.Helpers
{
    internal static class ExceptionHelper
    {
        internal static IEnumerable<Exception> Unwrap(Exception exception)
        {
            Ensure.NotNull(exception, nameof(exception));

            for (var ex = exception; ex is null == false; ex = ex.InnerException)
            {
                yield return ex;
            }
        }

        internal static string GetAllMessages(Exception exception)
        {
            Ensure.NotNull(exception, nameof(exception));

            var exceptions = Unwrap(exception).ToArray();

            var messageBuilder = new StringBuilder()
                .AppendLine("An error has occured:")
                .Append("> ")
                .AppendLine(exceptions[0].Message);

            for (int i = 1; i < exceptions.Length; i++)
            {
                messageBuilder
                    .AppendMultiple('>', (uint)(i + 1))
                    .Append(' ')
                    .AppendLine(exceptions[i].Message);
            }

            return messageBuilder.ToString();
        }
    }
}
