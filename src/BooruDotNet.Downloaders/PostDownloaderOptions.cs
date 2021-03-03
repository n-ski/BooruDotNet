namespace BooruDotNet.Downloaders
{
    public class PostDownloaderOptions : DownloaderOptions
    {
        public PostDownloaderOptions(int batchSize = 1, bool overwriteExisting = true, bool ignoreArchiveFiles = false)
            : base(batchSize, overwriteExisting)
        {
            IgnoreArchiveFiles = ignoreArchiveFiles;
        }

        public bool IgnoreArchiveFiles { get; }
    }
}
