using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
                this.OneWayBind(
                    ViewModel,
                    vm => vm.SearchResults,
                    v => v.ResultsControl.ItemsSource)
                    .DisposeWith(d);

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
            });
        }
    }
}
