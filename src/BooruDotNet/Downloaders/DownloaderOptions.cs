using System;

namespace BooruDotNet.Downloaders
{
    public class DownloaderOptions
    {
        public DownloaderOptions(int batchSize = 1)
        {
            BatchSize = Math.Max(1, batchSize);
        }

        public int BatchSize { get; }
    }
}
