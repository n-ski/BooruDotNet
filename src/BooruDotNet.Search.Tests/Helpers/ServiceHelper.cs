using System;

namespace BooruDotNet.Search.Tests.Helpers
{
    internal static class ServiceHelper
    {
        internal static T CreateService<T>(Type type)
        {
            T service = (T)Activator.CreateInstance(type, BooruDotNet.Tests.Shared.BooruHelper.HttpClient)!;
            return service;
        }
    }
}
