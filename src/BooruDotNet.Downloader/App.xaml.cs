using System.Windows;
using BooruDotNet.Boorus;
using BooruDotNet.Downloader.ViewModels;
using BooruDotNet.Downloader.Views;
using BooruDotNet.Downloaders;
using BooruDotNet.Links;
using BooruDotNet.Namers;
using ReactiveUI;
using Splat;

namespace BooruDotNet.Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            // Register classes manually because generic reactive window is defined in this assembly.
            Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => new LinkInputView(), typeof(IViewFor<LinkInputViewModel>));
            Locator.CurrentMutable.Register(() => new QueueItemView(), typeof(IViewFor<QueueItemViewModel>));

            var danbooru = new Danbooru();
            var gelbooru = new Gelbooru();

            LinkResolver.RegisterResolver(new DanbooruResolver(danbooru));
            LinkResolver.RegisterResolver(new GelbooruResolver(gelbooru));

            PostNamer = new HashNamer();
            PostDownloader = new PostDownloader(SingletonHttpClient.Instance, PostNamer);
        }

        internal static IPostNamer PostNamer { get; } // TODO: needs to be a setting.
        internal static PostDownloader PostDownloader { get; }
    }
}
