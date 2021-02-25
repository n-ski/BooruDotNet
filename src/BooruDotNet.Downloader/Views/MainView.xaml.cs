using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BooruDotNet.Downloader.Helpers;
using BooruDotNet.Downloader.ViewModels;
using Humanizer;
using Microsoft.Win32;
using ReactiveUI;

namespace BooruDotNet.Downloader.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveFabricWindow<MainViewModel>
    {
        private string _initialDialogDirectory;

        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            #region Add posts busy content

            AddPostsBusyContent = (StackPanel)FindResource(nameof(AddPostsBusyContent));
            AddedItemsProgressLabel = (TextBlock)LogicalTreeHelper.FindLogicalNode(AddPostsBusyContent, nameof(AddedItemsProgressLabel));
            AddedItemsProgressBar = (ProgressBar)LogicalTreeHelper.FindLogicalNode(AddPostsBusyContent, nameof(AddedItemsProgressBar));
            CancelAddButton = (Button)LogicalTreeHelper.FindLogicalNode(AddPostsBusyContent, nameof(CancelAddButton));

            #endregion

            #region Download busy content

            DownloadBusyContent = (StackPanel)FindResource(nameof(DownloadBusyContent));
            DownloadedFilesProgressLabel = (TextBlock)LogicalTreeHelper.FindLogicalNode(DownloadBusyContent, nameof(DownloadedFilesProgressLabel));
            DownloadedFilesProgressBar = (ProgressBar)LogicalTreeHelper.FindLogicalNode(DownloadBusyContent, nameof(DownloadedFilesProgressBar));
            CancelDownloadButton = (Button)LogicalTreeHelper.FindLogicalNode(DownloadBusyContent, nameof(CancelDownloadButton));

            #endregion

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.IsBusy, v => v.BusyIndicator.IsBusy)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.QueuedItems, v => v.QueuedItemsListBox.ItemsSource)
                    .DisposeWith(d);

                // Since we can't observe SelectedItems directly, let's observe the event instead.
                QueuedItemsListBox
                    .Events().SelectionChanged
                    .Select(_ => QueuedItemsListBox.SelectedItems.Cast<QueueItemViewModel>())
                    .BindTo(this, v => v.ViewModel.SelectedItems)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsAddingPosts, v => v.AddButton.IsEnabled, isBusy => !isBusy)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddFromFile, v => v.AddFromFileMenuItem)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.RemoveSelection, v => v.RemoveSelectionButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ClearQueue, v => v.ClearQueueButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.DownloadPosts, v => v.DownloadButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.QueuedItems.Count,
                    v => v.DownloadButton.Content,
                    count => count > 0 ? $"Download {"file".ToQuantity(count)}" : "Download")
                    .DisposeWith(d);

                #region Global interactions

                Interactions.OpenFileBrowser.RegisterHandler(interaction =>
                {
                    if (!Directory.Exists(_initialDialogDirectory))
                    {
                        _initialDialogDirectory = Environment.CurrentDirectory;
                    }

                    var dialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = interaction.Input,
                        InitialDirectory = _initialDialogDirectory,
                        Multiselect = false,
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        var fileInfo = new FileInfo(dialog.FileName);

                        _initialDialogDirectory = fileInfo.DirectoryName;
                        interaction.SetOutput(fileInfo);
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                }).DisposeWith(d);

                Interactions.OpenFolderBrowser.RegisterHandler(interaction =>
                {
                    var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

                    if (dialog.ShowDialog() == true)
                    {
                        var directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                        interaction.SetOutput(directoryInfo);
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                }).DisposeWith(d);

                Interactions.ShowErrorMessage.RegisterHandler(interaction =>
                {
                    var message = ExceptionHelper.GetAllMessages(interaction.Input);

                    MessageHelper.Warning(message);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                Interactions.ShowWarning.RegisterHandler(interaction =>
                {
                    var message = interaction.Input;

                    MessageHelper.Warning(message);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                #endregion

                #region Add posts busy content

                this.OneWayBind(
                    ViewModel,
                    vm => vm.ProgressMaximum,
                    v => v.AddedItemsProgressLabel.Text,
                    total => $"Fetching posts from {"link".ToQuantity(total)}\u2026")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressValue, v => v.AddedItemsProgressBar.Value)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressMaximum, v => v.AddedItemsProgressBar.Maximum)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelAdd, v => v.CancelAddButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.IsAddingPosts,
                    v => v.BusyIndicator.BusyContent,
                    isAdding => isAdding ? AddPostsBusyContent : null)
                    .DisposeWith(d);

                #endregion

                #region Download busy content

                this.OneWayBind(
                    ViewModel,
                    vm => vm.ProgressMaximum,
                    v => v.DownloadedFilesProgressLabel.Text,
                    total => $"Downloading {"files".ToQuantity(total)}\u2026")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressValue, v => v.DownloadedFilesProgressBar.Value)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ProgressMaximum, v => v.DownloadedFilesProgressBar.Maximum)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.CancelDownload, v => v.CancelDownloadButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.IsDownloading,
                    v => v.BusyIndicator.BusyContent,
                    isDownloading => isDownloading ? DownloadBusyContent : null)
                    .DisposeWith(d);

                #endregion

                this.BindCommand(ViewModel, vm => vm.OpenSettingsCommand, v => v.SettingsButton)
                    .DisposeWith(d);

                ViewModel.OpenSettingsInteraction.RegisterHandler(interaction =>
                {
                    var settingsView = new SettingsView
                    {
                        Owner = this,
                    };

                    return Observable.Start(() =>
                    {
                        settingsView.ShowDialog();

                        interaction.SetOutput(Unit.Default);
                    }, RxApp.MainThreadScheduler);
                }).DisposeWith(d);
            });
        }

        #region Add posts busy content

        internal StackPanel AddPostsBusyContent { get; }
        internal TextBlock AddedItemsProgressLabel { get; }
        internal ProgressBar AddedItemsProgressBar { get; }
        internal Button CancelAddButton { get; }

        #endregion

        #region Download busy content

        internal StackPanel DownloadBusyContent { get; }
        internal TextBlock DownloadedFilesProgressLabel { get; }
        internal ProgressBar DownloadedFilesProgressBar { get; }
        internal Button CancelDownloadButton { get; }

        #endregion
    }
}
