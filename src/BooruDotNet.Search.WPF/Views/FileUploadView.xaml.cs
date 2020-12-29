using System;
using System.Reactive.Disposables;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using BooruDotNet.Search.WPF.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Views
{
    /// <summary>
    /// Interaction logic for FileUploadView.xaml
    /// </summary>
    public partial class FileUploadView : ReactiveUserControl<FileUploadViewModel>
    {
        public FileUploadView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(
                    ViewModel,
                    vm => vm.OpenFileCommand,
                    v => v.OpenFileButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.FileInfo,
                    v => v.FilePathTextBox.Text,
                    fileInfo => fileInfo?.FullName ?? "")
                    .DisposeWith(d);

                FilePathTextBox
                    .Events().GotFocus
                    // Text needs to be selected in background due to dispatcher timing issues.
                    .Subscribe(_ => Dispatcher.InvokeAsync(
                        FilePathTextBox.SelectAll,
                        DispatcherPriority.Background))
                    .DisposeWith(d);
            });
        }
    }
}
