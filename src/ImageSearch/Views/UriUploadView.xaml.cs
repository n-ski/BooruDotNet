#nullable disable
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.Views
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

                ImageUriTextBox
                    .Events().GotFocus
                    // Text needs to be selected in background due to dispatcher timing issues.
                    .Subscribe(_ => Dispatcher.InvokeAsync(
                        ImageUriTextBox.SelectAll,
                        DispatcherPriority.Background))
                    .DisposeWith(d);
            });
        }
    }
}
