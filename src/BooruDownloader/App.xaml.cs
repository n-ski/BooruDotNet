using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Windows;
using BooruDotNet;
using BooruDotNet.Boorus;
using BooruDotNet.Caching;
using BooruDotNet.Downloaders;
using BooruDotNet.Links;
using BooruDotNet.Namers;
using BooruDotNet.Posts;
using BooruDownloader.ViewModels;
using BooruDownloader.Views;
using ReactiveUI;
using Splat;

namespace BooruDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            var httpClient = new HttpClient(new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
            });

            var danbooru = new Danbooru(httpClient);
            var gelbooru = new Gelbooru(httpClient);

            LinkResolver.RegisterResolver(new DanbooruResolver(danbooru));
            LinkResolver.RegisterResolver(new GelbooruResolver(gelbooru));

            var tagCache = new TagCache(danbooru);

            Downloaders = new Dictionary<FileNamingStyle, DownloaderBase<IPost>>
            {
                [FileNamingStyle.Hash] = new PostDownloader(httpClient, new HashNamer()),
                [FileNamingStyle.DanbooruStrict] = new PostDownloader(httpClient, new DanbooruNamer(tagCache)),
                [FileNamingStyle.DanbooruFancy] = new PostDownloader(httpClient, new DanbooruFancyNamer(tagCache)),
            };

            // Register classes manually because generic reactive window is defined in this assembly.
            Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => new LinkInputView(), typeof(IViewFor<LinkInputViewModel>));
            Locator.CurrentMutable.Register(() => new QueueItemView(tagCache), typeof(IViewFor<QueueItemViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
            Locator.CurrentMutable.Register(() => new PostView(), typeof(IViewFor<PostViewModel>));
            Locator.CurrentMutable.Register(() => new MediaView(), typeof(IViewFor<MediaViewModel>));
            Locator.CurrentMutable.Register(() => new TagView(), typeof(IViewFor<TagViewModel>));
        }

        internal static IReadOnlyDictionary<FileNamingStyle, DownloaderBase<IPost>> Downloaders { get; }
    }
}
