using System;

namespace BooruDotNet.Downloaders
{
    public class DownloaderOptions
    {
        public DownloaderOptions(int batchSize = 1, bool overwriteExisting = true, bool ignoreErrors = false)
        {
            BatchSize = Math.Max(1, batchSize);
            OverwriteExisting = overwriteExisting;
            IgnoreErrors = ignoreErrors;
        }

        public int BatchSize { get; }

        public bool OverwriteExisting { get; set; }

        public bool IgnoreErrors { get; set; }
    }
}
