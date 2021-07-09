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
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(typeof(App).Assembly);
        }
    }
}
