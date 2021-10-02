using System.Windows;
using BooruDotNet.Boorus;
using BooruDotNet.Links;
using BooruDownloader.Singletons;
using BooruDownloader.ViewModels;
using BooruDownloader.WPF.Views;
using ReactiveUI;
using Splat;

namespace BooruDownloader.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            LinkResolver.RegisterResolver(new DanbooruResolver(Boorus.Danbooru));
            LinkResolver.RegisterResolver(new GelbooruResolver(Boorus.Gelbooru));
            LinkResolver.RegisterResolver(new KonachanResolver(Boorus.Konachan));
            LinkResolver.RegisterResolver(new SankakuComplexResolver(Boorus.SankakuComplex));
            LinkResolver.RegisterResolver(new YandereResolver(Boorus.Yandere));

            // Register classes manually because generic reactive window is defined in this assembly.
            Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
            Locator.CurrentMutable.Register(() => new LinkInputView(), typeof(IViewFor<LinkInputViewModel>));
            Locator.CurrentMutable.Register(() => new QueueItemView(TagCaches.DanbooruTagCache), typeof(IViewFor<QueueItemViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
            Locator.CurrentMutable.Register(() => new PostView(), typeof(IViewFor<PostViewModel>));
            Locator.CurrentMutable.Register(() => new MediaView(), typeof(IViewFor<MediaViewModel>));
            Locator.CurrentMutable.Register(() => new TagView(), typeof(IViewFor<TagViewModel>));
        }
    }
}
