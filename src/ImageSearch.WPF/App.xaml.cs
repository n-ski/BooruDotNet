using System.Net;
using System.Net.Http;
using System.Windows;
using ReactiveUI;
using Splat;

namespace ImageSearch.WPF
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
        }

        internal static HttpClient HttpClient { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(App).Assembly);

            // This must be called here to initialize App resources.
            Locator.CurrentMutable.RegisterConstant(SearchServices.Initialize(HttpClient));
        }
    }
}
