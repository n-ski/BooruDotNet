#nullable disable
using System.IO;
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;
using Splat;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for FileQueueItemView.xaml
    /// </summary>
    public partial class FileQueueItemListView : ReactiveUserControl<FileQueueItemViewModel>
    {
        public FileQueueItemListView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Thumbnail, v => v.ThumbnailImage.Source, bitmap => bitmap?.ToNative())
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ImageFilePath, v => v.FileNameTextBlock.Text, Path.GetFileName)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ImageFilePath, v => v.FileNameTextBlock.ToolTip)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel, v => v.StatusView.ViewModel)
                    .DisposeWith(d);
            });
        }
    }
}
