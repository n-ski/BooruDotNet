#nullable disable
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using BooruDotNet.Helpers;
using BooruDownloader.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace BooruDownloader.WPF.Views
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

                this.OneWayBind(ViewModel,
                    vm => vm.IsAnimatedMedia,
                    v => v.MediaAnimated.Source,
                    isAnimated => isAnimated ? ViewModel.Uri : null)
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

                this.OneWayBind(ViewModel,
                    vm => vm.IsStaticImage,
                    v => v.ImageStatic.Source,
                    isStatic => isStatic ? ImageHelper.CreateImageFromUri(ViewModel.Uri) : null)
                    .DisposeWith(d);

                #endregion
            });
        }
    }
}
