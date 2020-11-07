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
            Ensure.Not(item.FileUri is null, "File URI is null.");

            return item.FileUri!;
        }

        protected override string GetFileName(IPost item)
        {
            Ensure.NotNull(item, nameof(item));
            Ensure.Not(item.FileUri is null, "File URI is null.");

            return _postNamer.Name(item) + item.FileUri!.GetExtension();
        }
    }
}
