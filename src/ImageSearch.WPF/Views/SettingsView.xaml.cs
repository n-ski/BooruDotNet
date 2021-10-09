#nullable disable
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ImageSearch.ViewModels;
using ReactiveMarbles.ObservableEvents;
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
                var settings = ViewSettings.Default;
                ColorThemeComboBox.SelectedIndex = settings.ColorThemeIndex;
                ShowUploadTipCheckBox.IsChecked = settings.ShowUploadTooltip;

                this.WhenAnyValue(v => v.ColorThemeComboBox.SelectedIndex)
                    .Where(index => index > -1)
                    .BindTo(ViewSettings.Default, s => s.ColorThemeIndex)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.EnableFiltering, v => v.EnableFilteringCheckBox.IsChecked)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.EnableFiltering, v => v.MinSimilarityUpDown.IsEnabled)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.MinSimilarity, v => v.MinSimilarityUpDown.Value)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.EnableImageCompression, v => v.EnableCompressionCheckBox.IsChecked)
                    .DisposeWith(d);

                OkButton.Events()
                    .Click
                    .Select(_ => true)
                    .Do(_=>
                    {
                        settings.ShowUploadTooltip = ShowUploadTipCheckBox.IsChecked is true;
                    })
                    .BindTo(this, v => v.DialogResult)
                    .DisposeWith(d);
            });
        }
    }
}
