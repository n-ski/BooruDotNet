#nullable disable
using System.Reactive.Disposables;
using System.Windows;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(d =>
            {
                static Visibility intToVisibility(int n) => n > 0 ? Visibility.Visible : Visibility.Collapsed;

                #region Best results

                this.OneWayBind(ViewModel, vm => vm.BestResults.Count, v => v.BestSearchResultsTextBlock.Visibility, intToVisibility)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.BestResults, v => v.BestSearchResultsControl.ItemsSource)
                    .DisposeWith(d);

                #endregion

                #region Other results

                this.OneWayBind(ViewModel, vm => vm.OtherResults.Count, v => v.OtherSearchResultsTextBlock.Visibility, intToVisibility)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.OtherResults, v => v.OtherSearchResultsControl.ItemsSource)
                    .DisposeWith(d);

                #endregion

                this.OneWayBind(ViewModel, vm => vm.UploadMethods, v => v.UploadMethodsComboBox.ItemsSource)
                    .DisposeWith(d);

                // This has to be a 2-way binding because view model can also change selected method (see SearchForSimilarImpl).
                this.Bind(ViewModel, vm => vm.SelectedUploadMethod, v => v.UploadMethodsComboBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.UploadMethod, v => v.UploadMethodHost.ViewModel)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SearchServices, v => v.SearchServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                this.WhenAnyValue(v => v.SearchServicesComboBox.SelectedItem)
                    .BindTo(this, v => v.ViewModel.SelectedSearchService)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.Search, v => v.SearchButton)
                    .DisposeWith(d);

                // Because selection is bound from view to view model, initialize default selection after we're done binding here.
                UploadMethodsComboBox.SelectedIndex = 0;
                SearchServicesComboBox.SelectedIndex = 0;
            });
        }
    }
}
