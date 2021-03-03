using System;

namespace BooruDotNet.Downloaders
{
    public class DownloaderOptions
    {
        public DownloaderOptions(int batchSize = 1, bool overwriteExisting = true)
        {
            BatchSize = Math.Max(1, batchSize);
            OverwriteExisting = overwriteExisting;
        }

        public int BatchSize { get; }

        public bool OverwriteExisting { get; set; }
    }
}
