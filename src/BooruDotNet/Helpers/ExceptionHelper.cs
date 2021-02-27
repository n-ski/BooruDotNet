using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BooruDotNet.Resources;
using Validation;

namespace BooruDotNet.Helpers
{
    internal static class ExceptionHelper
    {
        internal static IEnumerable<Exception> Unwrap(Exception exception)
        {
            Assumes.NotNull(exception);

            for (Exception? ex = exception; ex is null == false; ex = ex.InnerException)
            {
                yield return ex;
            }
        }

        internal static string GetAllMessages(Exception exception)
        {
            Requires.NotNull(exception, nameof(exception));

            Exception[] exceptions = Unwrap(exception).ToArray();

            StringBuilder messageBuilder = new StringBuilder()
                .AppendLine(ErrorMessages.ExceptionStackHeader)
                .Append("> ")
                .AppendLine(exceptions[0].Message);

            for (int i = 1; i < exceptions.Length; i++)
            {
                messageBuilder
                    .Append(new string('>', i + 1))
                    .Append(' ')
                    .AppendLine(exceptions[i].Message);
            }

            return messageBuilder.ToString();
        }
    }
}
