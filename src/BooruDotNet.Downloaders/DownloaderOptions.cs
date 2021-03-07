using System;

namespace BooruDotNet.Downloaders
{
    public class DownloaderOptions
    {
        private int _batchSize;

        public DownloaderOptions()
        {
            BatchSize = 1;
        }

        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = Math.Max(1, value); }
        }

        public bool OverwriteExisting { get; set; } = true;

        public bool IgnoreErrors { get; set; } = false;
    }
}
