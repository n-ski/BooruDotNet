#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for QueueItemView.xaml
    /// </summary>
    public partial class QueueItemView : ReactiveUserControl<QueueItemViewModel>
    {
        public QueueItemView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.SearchResults, v => v.SearchResultsItemsControl.ItemsSource)
                    .DisposeWith(d);
            });
        }
    }
}
