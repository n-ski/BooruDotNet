using System;
using System.Net.Http;
using BooruDotNet.Extensions;
using BooruDotNet.Namers;
using BooruDotNet.Posts;
using Easy.Common;

namespace BooruDotNet.Downloaders
{
    public class PostDownloader : DownloaderBase<IPost>
    {
        private readonly IPostNamer _postNamer;

        public PostDownloader(HttpClient httpClient, IPostNamer postNamer) : base(httpClient)
        {
            _postNamer = Ensure.NotNull(postNamer, nameof(postNamer));
        }

        protected override Uri GetDownloadUri(IPost item)
        {
            Ensure.NotNull(item, nameof(item));

            return GetFileOrSampleUri(item);
        }

        protected override string GetFileName(IPost item)
        {
            Ensure.NotNull(item, nameof(item));

            Uri uri = GetFileOrSampleUri(item);

            return _postNamer.Name(item) + uri.GetExtension();
        }

        private Uri GetFileOrSampleUri(IPost post)
        {
            Ensure.Not(post.FileUri is null, "File URI is null.");

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
                        Ensure.Not(post.SampleImageUri is null, "Sample URI is null.");
                        uri = post.SampleImageUri!;
                        break;
                }
            }

            return uri;
        }
    }
}
