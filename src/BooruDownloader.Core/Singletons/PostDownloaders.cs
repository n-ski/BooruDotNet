using System;
using BooruDotNet.Downloaders;
using BooruDotNet.Namers;
using BooruDotNet.Posts;
using Validation;

namespace BooruDownloader.Singletons
{
    internal static class PostDownloaders
    {
        private static readonly Lazy<DownloaderBase<IPost>> _hash = new Lazy<DownloaderBase<IPost>>(
            () => new PostDownloader(SingletonHttpClient.Instance, new HashNamer()));

        private static readonly Lazy<DownloaderBase<IPost>> _danbooruStrict = new Lazy<DownloaderBase<IPost>>(
            () => new PostDownloader(SingletonHttpClient.Instance, new DanbooruNamer(TagCaches.DanbooruTagCache)));

        private static readonly Lazy<DownloaderBase<IPost>> _danbooruFancy = new Lazy<DownloaderBase<IPost>>(
            () => new PostDownloader(SingletonHttpClient.Instance, new DanbooruFancyNamer(TagCaches.DanbooruTagCache)));

        public static DownloaderBase<IPost> GetDownloader(FileNamingStyle fileNamingStyle)
        {
            return fileNamingStyle switch
            {
                FileNamingStyle.Hash => _hash.Value,
                FileNamingStyle.DanbooruStrict => _danbooruStrict.Value,
                FileNamingStyle.DanbooruFancy => _danbooruFancy.Value,
                _ => throw Assumes.NotReachable(),
            };
        }
    }
}
