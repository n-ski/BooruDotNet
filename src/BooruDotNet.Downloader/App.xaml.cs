using System.Windows;
using BooruDotNet.Boorus;
using BooruDotNet.Downloader.ViewModels;
using BooruDotNet.Downloader.Views;
using BooruDotNet.Links;
using ReactiveUI;
using Splat;

namespace BooruDotNet.Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Register classes manually because generic reactive window is defined in this assembly.
            Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => new LinkInputView(), typeof(IViewFor<LinkInputViewModel>));
            Locator.CurrentMutable.Register(() => new QueueItemView(), typeof(IViewFor<QueueItemViewModel>));

            LinkResolver.RegisterResolver(new DanbooruResolver(new Danbooru()));
            LinkResolver.RegisterResolver(new GelbooruResolver(new Gelbooru()));
        }
    }
}
