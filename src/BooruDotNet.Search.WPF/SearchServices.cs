using System;
using System.Windows.Media.Imaging;
using BooruDotNet.Search.Services;
using BooruDotNet.Search.WPF.Models;

namespace BooruDotNet.Search.WPF
{
    internal static class SearchServices
    {
        private static readonly Lazy<SearchServiceModel> _danbooruLazy = new Lazy<SearchServiceModel>(
            () => new SearchServiceModel(
                new DanbooruService(App.HttpClient),
                "Danbooru",
                (BitmapImage)App.Current.Resources["DanbooruIcon"]));

        private static readonly Lazy<SearchServiceModel> _danbooruIqdbLazy = new Lazy<SearchServiceModel>(
            () => new SearchServiceModel(
                new IqdbService(App.HttpClient, "danbooru"),
                "Danbooru (IQDB)",
                (BitmapImage)App.Current.Resources["DanbooruIcon"]));

        internal static SearchServiceModel Danbooru => _danbooruLazy.Value;
        internal static SearchServiceModel DanbooruIqdb => _danbooruIqdbLazy.Value;
    }
}
