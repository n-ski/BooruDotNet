#nullable disable
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for SearchResultGroupView.xaml
    /// </summary>
    public partial class SearchResultGroupView : ReactiveUserControl<SearchResultGroupViewModel>
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(SearchResultGroupView),
            new PropertyMetadata(string.Empty, HeaderPropertyChangedCallback));

        public SearchResultGroupView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.SearchResults, v => v.SearchResultsControl.ItemsSource)
                    .DisposeWith(d);

                // Collapse this entire control if there's no results.
                this.OneWayBind(ViewModel, vm => vm.HasResults, v => v.Visibility)
                    .DisposeWith(d);
            });
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private static void HeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SearchResultGroupView)d;
            view.HeaderTextBlock.SetValue(TextBlock.TextProperty, e.NewValue);
        }
    }
}
