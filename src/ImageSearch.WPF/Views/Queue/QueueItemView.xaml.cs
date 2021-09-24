#nullable disable
using System.Linq;
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
                this.WhenAnyValue(
                    v => v.ViewModel,
                    v => v.ViewModel.SearchResults,
                    (vm, results) => vm is object ? results : Enumerable.Empty<SearchResultViewModel>())
                    .BindTo(this, v => v.SearchResultsItemsControl.ItemsSource)
                    .DisposeWith(d);
            });
        }
    }
}
