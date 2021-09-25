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
        private const long _validSizeForCompression = 2 << 20; // 2 MiB.
        private const float _compressedImageHeight = 400;

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

            FileStream? fileToUpload = null;

            try
            {
                if (ApplicationSettings.Default.EnableImageCompression
                    && _imageFileInfo.Length > _validSizeForCompression) // 2 MiB
                {
                    StatusViewModel.Text = "Compressing image\u2026";

                    IBitmap? bitmap;

                    using (Stream fileStream = _imageFileInfo.OpenRead())
                    {
                        bitmap = await BitmapLoader.Current.Load(fileStream, default, _compressedImageHeight);
                    }

                    if (bitmap is object)
                    {
                        fileToUpload = File.Create(Path.GetTempFileName(), 0x1000, FileOptions.DeleteOnClose);

                        await bitmap.Save(CompressedBitmapFormat.Png, 1.0f, fileToUpload);

                        fileToUpload.Position = 0;
                    }
                }

                fileToUpload ??= _imageFileInfo.OpenRead();

                StatusViewModel.Text = "Please wait\u2026";

                var results = await service.SearchAsync(fileToUpload, ct);

                return results.Select(x => new SearchResultViewModel(x));
            }
            finally
            {
                fileToUpload?.Dispose();
            }
        }
    }
}
