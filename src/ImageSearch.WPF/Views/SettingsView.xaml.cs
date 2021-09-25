#nullable disable
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using ImageSearch.ViewModels;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : ReactiveWindow<SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.EnableFiltering, v => v.EnableFilteringCheckBox.IsChecked)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.EnableFiltering, v => v.MinSimilarityUpDown.IsEnabled)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.MinSimilarity, v => v.MinSimilarityUpDown.Value)
                    .DisposeWith(d);

                OkButton.Events()
                    .Click
                    .Select(_ => true)
                    .BindTo(this, v => v.DialogResult)
                    .DisposeWith(d);
            });
        }
    }
}
