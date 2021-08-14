using System;
using Validation;

namespace BooruDotNet.Search.Attributes
{
    internal sealed class IqdbImageBoardIdAttribute : Attribute
    {
        public IqdbImageBoardIdAttribute(string serviceId)
        {
            Assumes.NotNullOrEmpty(serviceId);

            ServiceId = serviceId;
        }

        public string ServiceId { get; }
    }
}
