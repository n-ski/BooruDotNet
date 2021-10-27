using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using BooruDotNet.Search.Services;
using ImageSearch.ViewModels;

namespace ImageSearch
{
    internal static class SearchServices
    {
        internal static ReadOnlyCollection<SearchServiceViewModel> Initialize(HttpClient httpClient)
        {
            Debug.Assert(httpClient is object);

            SearchServiceViewModel[] services =
            {
                new SearchServiceViewModel(
                    new DanbooruService(httpClient),
                    "Danbooru",
                    "ImageSearch.Resources.ServiceIcons.Danbooru.png"),
                new SearchServiceViewModel(
                    new IqdbService(httpClient),
                    "IQDB (multi-service)",
                    "ImageSearch.Resources.ServiceIcons.IQDB.png"),
                new SearchServiceViewModel(
                    new IqdbService(httpClient, "gelbooru"),
                    "Gelbooru",
                    "ImageSearch.Resources.ServiceIcons.Gelbooru.png"),
            };

            return Array.AsReadOnly(services);
        }
    }
}
