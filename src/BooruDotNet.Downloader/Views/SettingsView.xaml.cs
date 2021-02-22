using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Downloader.ViewModels;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : ReactiveFabricWindow<SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();
            ViewModel = new SettingsViewModel();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.BatchSizes, v => v.BatchSizeComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.BatchSize, v => v.BatchSizeComboBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.FileNamingStyles, v => v.FileNamingStyleComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FileNamingStyle, v => v.FileNamingStyleComboBox.SelectedItem)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SaveSettings, v => v.OkButton)
                    .DisposeWith(d);

                OkButton
                    .Events().Click
                    .Do(_ => DialogResult = true)
                    .Subscribe()
                    .DisposeWith(d);

                CancelButton
                    .Events().Click
                    .Do(_ => DialogResult = false)
                    .Subscribe()
                    .DisposeWith(d);
            });
        }
    }
}
