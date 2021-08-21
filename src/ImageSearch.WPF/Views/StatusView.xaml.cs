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
                // Since this view never leaves visual tree, progress bar will never stop animating (and thus will waste processing power).
                // So when we aren't active, disable the animation by changing the progress bar style.
                this.OneWayBind(ViewModel, vm => vm.IsActive, v => v.StatusProgressBar.IsIndeterminate)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StatusText, v => v.StatusTextBlock.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelOperation, v => v.CancelButton)
                    .DisposeWith(d);
            });
        }
    }
}
