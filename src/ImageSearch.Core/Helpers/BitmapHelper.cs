﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Splat;

namespace ImageSearch.Helpers
{
    internal static class BitmapHelper
    {
        public static async Task<IBitmap> LoadBitmapAsync(FileInfo file, float? width, float? height)
        {
            Debug.Assert(file is object);
            Debug.Assert(file.Exists);

            IBitmap? bitmap;

            try
            {
                using Stream stream = file.OpenRead();
                bitmap = await BitmapLoader.Current.Load(stream, width, height);
            }
            catch
            {
                bitmap = null;
            }

            return bitmap ?? await LoadErrorBitmap();
        }

        public static async Task<IBitmap> LoadBitmapAsync(Uri uri, float? width, float? height)
        {
            Debug.Assert(uri is object);

            IBitmap? bitmap;

            try
            {
                using var wc = new WebClient();
                using Stream stream = wc.OpenRead(uri);

                // Splat has a bug with unfreezable images so copy the stream to memory first.
                Stream memory = new MemoryStream();
                await stream.CopyToAsync(memory);
                memory.Position = 0;

                bitmap = await BitmapLoader.Current.Load(memory, width, height);
            }
            catch
            {
                bitmap = null;
            }

            return bitmap ?? await LoadErrorBitmap();
        }

        private static async Task<IBitmap> LoadErrorBitmap()
        {
            Stream memory = new MemoryStream(Resources.ImageError);

            IBitmap? bitmap = await BitmapLoader.Current.Load(memory, default, default);

            return bitmap!;
        }
    }
}