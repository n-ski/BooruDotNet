using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BooruDotNet.Resources;

namespace BooruDotNet.Helpers;

internal static class ExceptionHelper
{
    internal static IEnumerable<Exception> Unwrap(Exception exception)
    {
        Debug.Assert(exception is object);

        for (Exception? ex = exception; ex is object; ex = ex.InnerException)
        {
            yield return ex;
        }
    }

    internal static string GetAllMessages(Exception exception)
    {
        Debug.Assert(exception is object);

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
