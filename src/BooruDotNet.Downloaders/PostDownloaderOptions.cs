namespace BooruDotNet.Downloaders
{
    public class PostDownloaderOptions : DownloaderOptions
    {
        public PostDownloaderOptions(int batchSize = 1, bool overwriteExisting = true,
            bool ignoreErrors = false, bool ignoreArchiveFiles = false)
            : base(batchSize, overwriteExisting, ignoreErrors)
        {
            IgnoreArchiveFiles = ignoreArchiveFiles;
        }

        public bool IgnoreArchiveFiles { get; }
    }
}
