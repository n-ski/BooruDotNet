using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class UriQueueItemViewModel : QueueItemViewModel
    {
        public UriQueueItemViewModel(Uri imageUri)
        {
            ImageUri = Requires.NotNull(imageUri, nameof(imageUri));
        }

        public Uri ImageUri { get; }

        protected override async Task<IBitmap?> LoadThumbnailImpl()
        {
            using var wc = new WebClient();
            using Stream stream = wc.OpenRead(ImageUri);

            // Splat has a bug with unfreezable images so copy the stream to memory first.
            Stream memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            return await BitmapLoader.Current.Load(memory, default, DesiredThumbnailHeight);

        }

        protected override async Task<IEnumerable<SearchResultViewModel>> SearchImpl(IFileAndUriSearchService service, CancellationToken ct)
        {
            Debug.Assert(service is object);

            CurrentStatusSubject.OnNext("Please wait\u2026");

            var results = await service.SearchAsync(ImageUri, ct);

            return results.Select(x => new SearchResultViewModel(x));
        }
    }
}
