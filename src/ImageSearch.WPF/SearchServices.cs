using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Media.Imaging;
using BooruDotNet.Search.Services;
using ImageSearch.ViewModels;
using Splat;

namespace ImageSearch.WPF
{
    internal static class SearchServices
    {
        internal static IEnumerable<SearchServiceViewModel> Initialize(HttpClient httpClient)
        {
            Debug.Assert(httpClient is object);

            var builder = ImmutableArray.CreateBuilder<SearchServiceViewModel>();

            builder.Add(new SearchServiceViewModel(
                new DanbooruService(httpClient),
                "Danbooru",
                BitmapFromResource("DanbooruIcon")));

            builder.Add(new SearchServiceViewModel(
                new IqdbService(httpClient),
                "IQDB (multi-service)",
                BitmapFromResource("IqdbIcon")));

            builder.Add(new SearchServiceViewModel(
                new IqdbService(httpClient, "gelbooru"),
                "Gelbooru",
                BitmapFromResource("GelbooruIcon")));

            return builder.ToImmutable();
        }

        private static IBitmap BitmapFromResource(string resourceName)
        {
            var bitmapImage = (BitmapImage)App.Current.Resources[resourceName];

            Debug.Assert(bitmapImage is object, $"Resource not found: '{resourceName}'.");

            return bitmapImage.FromNative();
        }
    }
}
