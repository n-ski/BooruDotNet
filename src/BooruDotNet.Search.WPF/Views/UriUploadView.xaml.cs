using System.Reactive.Disposables;
using BooruDotNet.Search.WPF.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Views
{
    /// <summary>
    /// Interaction logic for UriUploadView.xaml
    /// </summary>
    public partial class UriUploadView : ReactiveUserControl<UriUploadViewModel>
    {
        public UriUploadView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(
                    ViewModel,
                    vm => vm.ImageUri,
                    v => v.ImageUriTextBox.Text)
                    .DisposeWith(d);
            });
        }
    }
}
