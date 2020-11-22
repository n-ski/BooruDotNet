using System.Net;
using System.Net.Http;
using System.Windows;
using ReactiveUI;
using Splat;

namespace BooruDotNet.Search.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            HttpClient = new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });

            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(App).Assembly);
        }

        internal static HttpClient HttpClient { get; }
    }
}
