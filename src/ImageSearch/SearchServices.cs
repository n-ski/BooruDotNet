using System;
using System.Collections.Immutable;
using System.Windows.Media.Imaging;
using BooruDotNet.Search.Services;
using ImageSearch.Models;

namespace ImageSearch
{
    internal static class SearchServices
    {
        private static readonly Lazy<ImmutableArray<SearchServiceModel>> _servicesLazy = 
            new Lazy<ImmutableArray<SearchServiceModel>>(() =>
            {
                var builder = ImmutableArray.CreateBuilder<SearchServiceModel>();

                builder.Add(new SearchServiceModel(
                    new IqdbService(App.HttpClient),
                    "IQDB (multi-service)",
                    (BitmapImage)App.Current.Resources["AppIcon_16px"]));

                builder.Add(new SearchServiceModel(
                    new DanbooruService(App.HttpClient),
                    "Danbooru",
                    (BitmapImage)App.Current.Resources["DanbooruIcon"]));

                builder.Add(new SearchServiceModel(
                    new IqdbService(App.HttpClient, "danbooru"),
                    "IQDB (Danbooru)",
                    (BitmapImage)App.Current.Resources["DanbooruIcon"]));

                builder.Add(new SearchServiceModel(
                    new IqdbService(App.HttpClient, "gelbooru"),
                    "IQDB (Gelbooru)",
                    (BitmapImage)App.Current.Resources["GelbooruIcon"]));

                return builder.ToImmutable();
            });

        internal static ImmutableArray<SearchServiceModel> Services => _servicesLazy.Value;
    }
}
