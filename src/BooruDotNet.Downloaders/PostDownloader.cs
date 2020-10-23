using System;
using System.Net.Http;
using BooruDotNet.Extensions;
using BooruDotNet.Namers;
using BooruDotNet.Posts;
using Validation;

namespace BooruDotNet.Downloaders
{
    public class PostDownloader : DownloaderBase<IPost>
    {
        private readonly IPostNamer _postNamer;

        public PostDownloader(HttpClient httpClient, IPostNamer postNamer) : base(httpClient)
        {
            _postNamer = Requires.NotNull(postNamer, nameof(postNamer));
        }

        protected override Uri GetDownloadUri(IPost item)
        {
            Requires.NotNull(item, nameof(item));

            return GetFileOrSampleUri(item);
        }

        protected override string GetFileName(IPost item)
        {
            Requires.NotNull(item, nameof(item));

            Uri uri = GetFileOrSampleUri(item);

            return _postNamer.Name(item) + uri.GetExtension();
        }

        private Uri GetFileOrSampleUri(IPost post)
        {
            Verify.Operation(post.FileUri is object, "File URI is null.");

            Uri uri = post.FileUri!;

            // Prefer samples over archive files if the option to do so is enabled.
            if (Options is PostDownloaderOptions options
                && options.IgnoreArchiveFiles)
            {
                string extension = uri.GetExtension().ToLowerInvariant();

                switch (extension)
                {
                    case ".7z":
                    case ".rar":
                    case ".zip":
                        Verify.Operation(post.SampleImageUri is object, "Sample URI is null.");
                        uri = post.SampleImageUri!;
                        break;
                }
            }

            return uri;
        }
    }
}
