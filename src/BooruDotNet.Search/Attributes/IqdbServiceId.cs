using System;
using System.Diagnostics;

namespace BooruDotNet.Search.Attributes
{
    internal sealed class IqdbImageBoardIdAttribute : Attribute
    {
        public IqdbImageBoardIdAttribute(string serviceId)
        {
            Debug.Assert(string.IsNullOrWhiteSpace(serviceId) is false);

            ServiceId = serviceId;
        }

        public string ServiceId { get; }
    }
}
