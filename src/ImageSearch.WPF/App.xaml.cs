using System.Net;
using System.Net.Http;
using System.Windows;
using ImageSearch.ViewModels;
using ImageSearch.WPF.Views;
using ReactiveUI;
using Splat;
using TomsToolbox.Wpf.Styles;

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

            Locator.CurrentMutable.RegisterPlatformBitmapLoader();

            Locator.CurrentMutable.Register(() => new SearchResultView(), typeof(IViewFor<SearchResultViewModel>));
            Locator.CurrentMutable.Register(() => new QueueItemView(), typeof(IViewFor<QueueItemViewModel>));
            Locator.CurrentMutable.Register(() => new FileQueueItemListView(), typeof(IViewFor<FileQueueItemViewModel>), ViewContracts.QueueList);
            Locator.CurrentMutable.Register(() => new UriQueueItemListView(), typeof(IViewFor<UriQueueItemViewModel>), ViewContracts.QueueList);
        }

        internal static HttpClient HttpClient { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            WpfStyles.RegisterDefaultStyles(Resources).RegisterDefaultWindowStyle();

            // This must be called here because resources aren't initialized yet in static ctor.
            Locator.CurrentMutable.RegisterConstant(SearchServices.Initialize(HttpClient));
        }
    }
}
