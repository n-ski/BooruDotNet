using System.Collections.Generic;
using System.Windows;
using BooruDotNet.Boorus;
using BooruDotNet.Caches;
using BooruDotNet.Downloader.ViewModels;
using BooruDotNet.Downloader.Views;
using BooruDotNet.Downloaders;
using BooruDotNet.Links;
using BooruDotNet.Namers;
using BooruDotNet.Posts;
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
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));

            var danbooru = new Danbooru();
            var gelbooru = new Gelbooru();

            LinkResolver.RegisterResolver(new DanbooruResolver(danbooru));
            LinkResolver.RegisterResolver(new GelbooruResolver(gelbooru));

            var httpClient = SingletonHttpClient.Instance;
            var tagCache = new TagCache(danbooru);

            Downloaders = new Dictionary<FileNamingStyle, DownloaderBase<IPost>>
            {
                [FileNamingStyle.Hash] = new PostDownloader(httpClient, new HashNamer()),
                [FileNamingStyle.DanbooruStrict] = new PostDownloader(httpClient, new DanbooruNamer(tagCache)),
                [FileNamingStyle.DanbooruFancy] = new PostDownloader(httpClient, new DanbooruFancyNamer(tagCache)),
            };
        }

        internal static IReadOnlyDictionary<FileNamingStyle, DownloaderBase<IPost>> Downloaders { get; }
    }
}
