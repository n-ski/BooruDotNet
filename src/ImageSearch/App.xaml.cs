using System.Net;
using System.Net.Http;
using System.Windows;
using PresentationTheme.Aero;
using ReactiveUI;
using Splat;

namespace ImageSearch
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

            AeroTheme.SetAsCurrentTheme();
        }

        internal static HttpClient HttpClient { get; }
    }
}
