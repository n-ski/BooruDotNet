using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BooruDotNet
{
    internal sealed class SingletonHttpClient : HttpClient
    {
        private const int _maxConnectionsPerServer = 5;
        private static readonly Lazy<SingletonHttpClient> _instanceLazy =
            new Lazy<SingletonHttpClient>(() => new SingletonHttpClient());

        private SingletonHttpClient()
#if NETCOREAPP
            : base(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = _maxConnectionsPerServer,
            })
#else
            : base(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = _maxConnectionsPerServer,
            })
#endif
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
