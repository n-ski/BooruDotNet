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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(App).Assembly);
        }
    }
}
