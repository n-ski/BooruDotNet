using System.Reactive.Disposables;
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
                    vm => vm.SampleText,
                    v => v.SampleLabel.Content)
                    .DisposeWith(d);
            });
        }
    }
}
