using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Services;
using ImageSearch.Helpers;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class FileQueueItemViewModel : QueueItemViewModel
    {
        private readonly FileInfo _imageFileInfo;

        public FileQueueItemViewModel(FileInfo imageFileInfo)
        {
            _imageFileInfo = Requires.NotNull(imageFileInfo, nameof(imageFileInfo));
        }

        public string ImageFilePath => _imageFileInfo.FullName;

        protected override Task<IBitmap> LoadThumbnailImpl()
        {
            return BitmapHelper.LoadBitmapAsync(_imageFileInfo, default, DesiredThumbnailHeight);
        }

        protected override async Task<IEnumerable<SearchResultViewModel>> SearchImpl(IFileAndUriSearchService service, CancellationToken ct)
        {
            Debug.Assert(service is object);

            StatusViewModel.Text = "Please wait\u2026";

            using FileStream fileStream = _imageFileInfo.OpenRead();

            var results = await service.SearchAsync(fileStream, ct);

            return results.Select(x => new SearchResultViewModel(x));
        }
    }
}
