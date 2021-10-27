using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using ImageSearch.Helpers;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class UriQueueItemViewModel : QueueItemViewModel
    {
        private readonly HttpClient _httpClient;

        public UriQueueItemViewModel(Uri imageUri, HttpClient httpClient)
        {
            _httpClient = Requires.NotNull(httpClient, nameof(httpClient));
            ImageUri = Requires.NotNull(imageUri, nameof(imageUri));
        }

        public Uri ImageUri { get; }

        protected override Task<IBitmap> LoadThumbnailImpl()
        {
            return BitmapHelper.LoadBitmapAsync(ImageUri, _httpClient, default, DesiredThumbnailHeight);
        }

        protected override async Task<IEnumerable<SearchResultViewModel>> SearchImpl(IFileAndUriSearchService service, CancellationToken ct)
        {
            Debug.Assert(service is object);

            StatusViewModel.Text = "Please wait\u2026";

            var results = await service.SearchAsync(ImageUri, ct);

            return results.Select(x => new SearchResultViewModel(x));
        }
    }
}
