using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BooruDotNet
{
    internal sealed class SingletonHttpClient : HttpClient
    {
        private static readonly Lazy<SingletonHttpClient> _instanceLazy =
            new Lazy<SingletonHttpClient>(() => new SingletonHttpClient());

        private SingletonHttpClient()
            : base(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = 5,
            })
        {
            var assemblyName = typeof(SingletonHttpClient).Assembly.GetName();

            DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(
                    assemblyName.Name!,
                    assemblyName.Version!.ToString(3)));
        }

        public static SingletonHttpClient Instance => _instanceLazy.Value;
    }
}
