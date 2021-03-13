using System;
using System.Reactive.Disposables;
using System.Windows.Media.Imaging;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.Views
{
    /// <summary>
    /// Interaction logic for ResultView.xaml
    /// </summary>
    public partial class ResultView : ReactiveUserControl<ResultViewModel>
    {
        public ResultView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(
                    ViewModel,
                    vm => vm.ImageUri,
                    v => v.PreviewImage.Source,
                    uri => CreateFrom(uri))
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SourceUri.Host,
                    v => v.SiteImage.Source,
                    host => CreateFrom(new Uri($"https://www.google.com/s2/favicons?domain={host}")))
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SourceUri,
                    v => v.SiteImage.ToolTip)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.ImageSize,
                    v => v.ImageSizeText.Text,
                    size => $"{size.Width}x{size.Height}")
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.Similarity,
                    v => v.SimilarityText.Text,
                    similarity => $"{similarity:P0} similarity");

                this.BindCommand(
                    ViewModel,
                    vm => vm.OpenSourceCommand,
                    v => v.OpenSourceButton)
                    .DisposeWith(d);

                this.BindCommand(
                    ViewModel,
                    vm => vm.CopySourceUriCommand,
                    v => v.CopySourceButton)
                    .DisposeWith(d);

                this.BindCommand(
                    ViewModel,
                    vm => vm.SearchForSimilarCommand,
                    v => v.SearchButton)
                    .DisposeWith(d);
            });
        }

        private static BitmapImage CreateFrom(Uri uri)
        {
            if (uri is null)
            {
                return null;
            }

            var bitmapImage = new BitmapImage(uri);

            if (bitmapImage.CanFreeze)
            {
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
