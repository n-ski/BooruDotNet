namespace BooruDotNet.Downloaders
{
    public class PostDownloaderOptions : DownloaderOptions
    {
        public PostDownloaderOptions(int batchSize = 1, bool ignoreArchiveFiles = false)
            : base(batchSize)
        {
            IgnoreArchiveFiles = ignoreArchiveFiles;
        }

        public bool IgnoreArchiveFiles { get; }
    }
}
