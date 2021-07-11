#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for StatusView.xaml
    /// </summary>
    public partial class StatusView : ReactiveUserControl<StatusViewModel>
    {
        public StatusView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.StatusText, v => v.StatusTextBlock.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelOperation, v => v.CancelButton)
                    .DisposeWith(d);
            });
        }
    }
}
