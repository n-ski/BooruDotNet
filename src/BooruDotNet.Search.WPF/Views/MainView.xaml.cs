using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BooruDotNet.Search.WPF.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Views
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
                #region Results bindings

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SearchResultsBestMatches,
                    v => v.BestResultsControl.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SearchResultsOtherMatches,
                    v => v.OtherResultsControl.ItemsSource)
                    .DisposeWith(d);

                #endregion

                #region Best results visibility

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasBestResults,
                    v => v.BestResultsHeader.Visibility)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasBestResults,
                    v => v.BestResultsControl.Visibility)
                    .DisposeWith(d);

                #endregion

                #region Other results visibility

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasOtherResults,
                    v => v.OtherResultsHeader.Visibility)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasOtherResults,
                    v => v.OtherResultsControl.Visibility)
                    .DisposeWith(d);

                #endregion

                // Scroll to top when the results change.
                ViewModel
                    .WhenAnyValue(vm => vm.SearchResults)
                    .Subscribe(_ => ResultsScrollViewer.ScrollToTop())
                    .DisposeWith(d);

                this.Bind(
                    ViewModel,
                    vm => vm.SearchUri,
                    v => v.SearchUriTextBox.Text)
                    .DisposeWith(d);

                this.BindCommand(
                    ViewModel,
                    vm => vm.SearchCommand,
                    v => v.SearchButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.IsSearching,
                    v => v.SearchBusyIndicator.IsBusy)
                    .DisposeWith(d);

                SearchUriTextBox
                    .Events().KeyDown
                    .Where(e => e.Key == Key.Enter)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.SearchCommand)
                    .DisposeWith(d);
            });
        }
    }
}
