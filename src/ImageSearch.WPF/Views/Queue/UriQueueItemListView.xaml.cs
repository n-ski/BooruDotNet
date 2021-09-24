#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;
using Splat;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for UriQueueItemView.xaml
    /// </summary>
    public partial class UriQueueItemListView : ReactiveUserControl<UriQueueItemViewModel>
    {
        public UriQueueItemListView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Thumbnail, v => v.ThumbnailImage.Source, bitmap => bitmap?.ToNative())
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ImageUri, v => v.ImageUriTextBlock.Text, uri => uri.Segments[^1])
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ImageUri, v => v.ImageUriTextBlock.ToolTip)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel, v => v.StatusView.ViewModel)
                    .DisposeWith(d);
            });
        }
    }
}
