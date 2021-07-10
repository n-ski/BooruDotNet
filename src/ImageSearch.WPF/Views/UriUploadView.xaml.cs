#nullable disable
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

                UriTextBox.Events().KeyDown
                    .Where(args => args.Key is Key.Enter)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.Search);
            });
        }
    }
}
