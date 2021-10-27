#nullable disable
using System.Reactive.Disposables;
using ImageSearch.ViewModels;
using ReactiveUI;
using Splat;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for SearchServiceView.xaml
    /// </summary>
    public partial class SearchServiceView : ReactiveUserControl<SearchServiceViewModel>
    {
        public SearchServiceView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Icon, v => v.ServiceIcon.Source, bitmap => bitmap?.ToNative())
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.ServiceName.Text)
                    .DisposeWith(d);
            });
        }
    }
}
