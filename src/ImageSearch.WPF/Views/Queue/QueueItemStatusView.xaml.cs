#nullable disable
using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for QueueItemStatusViewModel.xaml
    /// </summary>
    public partial class QueueItemStatusView : ReactiveUserControl<QueueItemStatusViewModel>
    {
        public QueueItemStatusView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(
                    ViewModel,
                    vm => vm.Status,
                    v => v.StatusIcon.Child,
                    status => status switch
                    {
                        QueueItemStatus.Processing => App.Current.Resources["StatusProcessingIcon"],
                        QueueItemStatus.Complete => App.Current.Resources["StatusCompleteIcon"],
                        QueueItemStatus.Error => App.Current.Resources["StatusErrorIcon"],
                        _ => null,
                    })
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Text, v => v.StatusText.Text)
                    .DisposeWith(d);

                // Set custom tooltip if we've got an exception.
                this.WhenAnyValue(
                    v => v.ViewModel.Exception,
                    ex => ex is object
                        ? string.Join(
                            Environment.NewLine,
                            $"An exception of type '{ex.GetType()}' has occurred with the following message:",
                            ex.Message)
                        : null)
                    .BindTo(this, v => v.StatusText.ToolTip)
                    .DisposeWith(d);

                // Also set "help" cursor if we've got an exception.
                this.OneWayBind(
                    ViewModel,
                    vm => vm.Exception,
                    v => v.StatusText.Cursor,
                    ex => ex is object ? Cursors.Help : null)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.Retry, v => v.RetryButton)
                    .DisposeWith(d);

                // Show Retry button when we've got an exception
                this.OneWayBind(
                    ViewModel,
                    vm => vm.Exception,
                    v => v.RetryButton.Visibility,
                    ex => ex is object ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);
            });
        }
    }
}
