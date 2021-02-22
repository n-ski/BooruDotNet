using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Downloader.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for MediaView.xaml
    /// </summary>
    public partial class MediaView : ReactiveUserControl<MediaViewModel>
    {
        public MediaView()
        {
            InitializeComponent();
            ViewModel = new MediaViewModel();

            this.WhenActivated(d =>
            {
                #region Animated media bindings

                this.OneWayBind(ViewModel, vm => vm.IsAnimatedMedia, v => v.MediaAnimated.Visibility)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.ViewModel.IsAnimatedMedia)
                    .Where(IsTrue)
                    .Select(_ => ViewModel.Uri)
                    .BindTo(this, x => x.MediaAnimated.Source)
                    .DisposeWith(d);

                // Loop animation.
                MediaAnimated.Events()
                    .MediaEnded
                    .Do(args =>
                    {
                        var mediaElement = (MediaElement)args.OriginalSource;

                        // Fixes broken GIFs.
                        var time = mediaElement.Source.AbsoluteUri.EndsWith(".gif")
                            ? TimeSpan.FromMilliseconds(1)
                            : TimeSpan.Zero;

                        mediaElement.Position = time;
                    })
                    .Subscribe()
                    .DisposeWith(d);

                #endregion

                #region Static image bindings

                this.OneWayBind(ViewModel, vm => vm.IsStaticImage, v => v.ImageStatic.Visibility)
                    .DisposeWith(d);

                this.WhenAnyValue(x => x.ViewModel.IsStaticImage)
                    .Where(IsTrue)
                    .Select(_ => ImageHelper.CreateImageFromUri(ViewModel.Uri))
                    .BindTo(this, x => x.ImageStatic.Source)
                    .DisposeWith(d);

                #endregion
            });
        }

        private static bool IsTrue(bool b) => b;
    }
}
