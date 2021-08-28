#nullable disable
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls.Primitives;
using BooruDownloader.ViewModels;
using ReactiveUI;

namespace BooruDownloader.Views
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
                this.Bind(ViewModel, vm => vm.BatchSize, v => v.BatchSizeUpDown.Value)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.FileNamingStyles, v => v.FileNamingStyleComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.FileNamingStyle, v => v.FileNamingStyleComboBox.SelectedItem)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IgnoreArchiveFiles, v => v.IgnoreArchiveFilesCheckBox.IsChecked)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.NotifyAboutSkippedPosts, v => v.NotifyAboutSkippedPostsCheckBox.IsChecked)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.PlaySoundWhenComplete, v => v.PlaySoundWhenCompleteCheckBox.IsChecked)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.OverwriteExistingFiles, v => v.OverwriteExistingFilesCheckBox.IsChecked)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.IgnoreDownloadErrors, v => v.IgnoreDownloadErrorsCheckBox.IsChecked)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DownloadLocation, v => v.DownloadLocationTextBox.Text)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ChangeDownloadLocation, v => v.ChangeDownloadLocationButton)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AskLocationBeforeDownload, v => v.DownloadLocationTextBox.IsEnabled, ask => !ask)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.AskLocationBeforeDownload, v => v.AskDownloadLocationCheckBox.IsChecked)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SaveSettings, v => v.OkButton)
                    .DisposeWith(d);

                this.WhenAnyObservable(v => v.ViewModel.SaveSettings)
                    .Subscribe(_ => DialogResult = true)
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
