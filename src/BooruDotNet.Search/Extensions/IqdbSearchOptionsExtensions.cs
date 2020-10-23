using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BooruDotNet.Search.Attributes;
using BooruDotNet.Search.Services;

namespace BooruDotNet.Search.Extensions
{
    internal static class IqdbSearchOptionsExtensions
    {
        public static bool TryGetServiceId(this IqdbSearchOptions value, [NotNullWhen(true)] out string? serviceId)
        {
            var fieldInfo = typeof(IqdbSearchOptions).GetField(value.ToString());
            var attribute = fieldInfo?.GetCustomAttribute<IqdbImageBoardIdAttribute>();

            if (attribute is object)
            {
                serviceId = attribute.ServiceId;
                return true;
            }
            else
            {
                serviceId = null;
                return false;
            }
        }
    }
}
