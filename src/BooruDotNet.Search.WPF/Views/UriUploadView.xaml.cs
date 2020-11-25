using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

                ImageUriTextBox
                    .Events().KeyDown
                    .Where(e => e.Key == Key.Return)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.SearchCommand)
                    .DisposeWith(d);
            });
        }
    }
}
