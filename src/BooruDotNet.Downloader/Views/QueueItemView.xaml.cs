using System;
using System.Reactive.Disposables;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BooruDotNet.Downloader.ViewModels;
using Humanizer;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for QueueItemView.xaml
    /// </summary>
    public partial class QueueItemView : ReactiveUserControl<QueueItemViewModel>
    {
        public QueueItemView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PreviewImageUri, v => v.PreviewImage.Source, CreateImageFromUri)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SourceUri, v => v.SourceTextBlock.Text, StringifySource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.FileSize, v => v.FileSizeTextBlock.Text, StringifyFileSize)
                    .DisposeWith(d);
            });
        }

        private static ImageSource CreateImageFromUri(Uri uri)
        {
            var bitmapImage = new BitmapImage(uri);

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        private static string StringifySource(Uri uri)
        {
            return $"Source: {uri.Host}";
        }

        private static string StringifyFileSize(long? size)
        {
            return $"File size: {(size.HasValue ? size.Value.Bytes().Humanize("0.00") : "unknown")}";
        }
    }
}
