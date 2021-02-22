using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Downloader.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for PostView.xaml
    /// </summary>
    public partial class PostView : ReactiveFabricWindow<PostViewModel>
    {
        public PostView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Do(vm =>
                    {
                        CalculateWindowAspectRatio(vm.Post.Width, vm.Post.Height);
                    })
                    .Subscribe(ReactiveHelper.DoNothing)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.SampleImageUri, v => v.PostPreviewImage.Source, ImageHelper.CreateImageFromUri)
                    .DisposeWith(d);

                CloseButton
                    .Events().Click
                    .Do(_ => DialogResult = false)
                    .Subscribe(ReactiveHelper.DoNothing)
                    .DisposeWith(d);
            });
        }

        private void CalculateWindowAspectRatio(int width, int height)
        {
            const int largerSide = 600;
            double ratio = (double)width / height;

            if (ratio > 1)
            {
                Width = largerSide;
                Height = Convert.ToInt32(largerSide / ratio);
            }
            else
            {
                Width = Convert.ToInt32(largerSide * ratio);
                Height = largerSide;
            }
        }
    }
}
