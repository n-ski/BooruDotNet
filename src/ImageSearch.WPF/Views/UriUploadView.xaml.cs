#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for UriUploadViewModel.xaml
    /// </summary>
    public partial class UriUploadView : ReactiveUserControl<UriUploadViewModel>
    {
        public UriUploadView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.FileUri, v => v.UriTextBox.Text)
                    .DisposeWith(d);
            });
        }
    }
}
