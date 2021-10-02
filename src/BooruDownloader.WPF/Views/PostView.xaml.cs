#nullable disable
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using BooruDownloader.ViewModels;
using Humanizer;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace BooruDownloader.WPF.Views
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
                    .Subscribe()
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Tags, v => v.TagsItemsControl.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Post.SampleImageUri, v => v.MediaPreview.ViewModel.Uri)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Post.ID,
                    v => v.PostIdTextBlock.Text,
                    id => $"ID: {id}")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Post.CreationDate,
                    v => v.PostDateTextBlock.Text,
                    date => $"Date: {date.Humanize(utcDate: false)}")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Post.CreationDate,
                    v => v.PostDateTextBlock.ToolTip,
                    date => date.ToString())
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.Post.FileSize,
                    v => v.PostSizeTextBlock.Text,
                    size => $"Size: {(size.HasValue ? size.Value.Bytes().Humanize("0.00") : "unknown")}")
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.OpenInBrowser, v => v.OpenInBrowserButton)
                    .DisposeWith(d);

                this.Events().KeyDown
                    .Where(e => e.Key == Key.Escape)
                    .Do(_ => Close())
                    .Subscribe()
                    .DisposeWith(d);
            });
        }

        private void CalculateWindowAspectRatio(int width, int height)
        {
            const int largerSide = 600;
            const int sidePanelWidth = 220;

            double ratio = (double)width / height;

            if (ratio > 1)
            {
                Width = sidePanelWidth + largerSide;
                Height = Convert.ToInt32(largerSide / ratio);
            }
            else
            {
                Width = sidePanelWidth + Convert.ToInt32(largerSide * ratio);
                Height = largerSide;
            }
        }
    }
}
