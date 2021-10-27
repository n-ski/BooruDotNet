using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Splat;

namespace ImageSearch.Helpers
{
    internal static class BitmapHelper
    {
        private static readonly Lazy<Task<IBitmap>> _errorBitmap = new Lazy<Task<IBitmap>>(async () =>
        {
            IBitmap bitmap = await LoadBitmapAsync("ImageSearch.Resources.ImageError.png", typeof(BitmapHelper).Assembly, default, default);

            return bitmap;
        });

        public static Task<IBitmap> LoadBitmapAsync(string resourceName, Assembly assembly, float? width, float? height)
        {
            Debug.Assert(string.IsNullOrEmpty(resourceName) is false);
            Debug.Assert(assembly is object);

            return LoadBitmapAsync(() => assembly.GetManifestResourceStream(resourceName), width, height);
        }

        public static Task<IBitmap> LoadBitmapAsync(FileInfo file, float? width, float? height)
        {
            Debug.Assert(file is object);
            Debug.Assert(file.Exists);

            return LoadBitmapAsync(() => file.OpenRead(), width, height);
        }

        public static Task<IBitmap> LoadBitmapAsync(Uri uri, HttpClient httpClient, float? width, float? height)
        {
            Debug.Assert(uri is object);
            Debug.Assert(httpClient is object);

            return LoadBitmapAsync(() => httpClient.GetStreamAsync(uri), width, height);
        }

        private static Task<IBitmap> LoadBitmapAsync(Func<Stream> func, float? width, float? height)
        {
            Debug.Assert(func is object);

            return LoadBitmapAsync(() => Task.Run(func), width, height);
        }

        private static async Task<IBitmap> LoadBitmapAsync(Func<Task<Stream>> func, float? width, float? height)
        {
            Debug.Assert(func is object);

            IBitmap? bitmap;

            try
            {
                using Stream stream = await func();

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

            return bitmap ?? await _errorBitmap.Value;
        }
    }
}
