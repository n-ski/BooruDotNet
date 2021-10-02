#nullable disable
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BooruDotNet.Helpers;
using BooruDownloader.Interactions;
using BooruDownloader.ViewModels;
using Humanizer;
using Microsoft.Win32;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace BooruDownloader.WPF.Views
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

                this.OneWayBind(ViewModel, vm => vm.SelectedItems, v => v.QueuedItemsListBox.SelectedItems)
                    .DisposeWith(d);

                // ListBox is trash and SelectedItems binding doesn't notify us about selected items changes,
                // so handle the event and manipulate the collection manually instead.
                QueuedItemsListBox
                    .Events().SelectionChanged
                    .Subscribe(args =>
                    {
                        var selectedItems = ViewModel.SelectedItems;

                        using (selectedItems.SuspendNotifications())
                        {
                            foreach (QueueItemViewModel item in args.RemovedItems)
                            {
                                selectedItems.Remove(item);
                            }

                            foreach (QueueItemViewModel item in args.AddedItems)
                            {
                                selectedItems.Add(item);
                            }
                        }
                    })
                    .DisposeWith(d);

                QueuedItemsListBox
                    .Events().KeyDown
                    .Where(e => e.Key == Key.Delete)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.RemoveSelection)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.IsAddingPosts, v => v.AddButton.IsEnabled, isBusy => !isBusy)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddFromUrls, v => v.AddFromUrlsMenuItem)
                    .DisposeWith(d);

                ViewModel.OpenUrlInputDialog.RegisterHandler(interaction =>
                {
                    var dialog = new LinkInputView
                    {
                        Owner = this
                    };

                    return Observable.Start(() =>
                    {
                        if (dialog.ShowDialog() == true)
                        {
                            interaction.SetOutput(dialog.ViewModel.Links);
                        }
                        else
                        {
                            interaction.SetOutput(null);
                        }
                    }, RxApp.MainThreadScheduler);
                }).DisposeWith(d);

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

                #region Message interactions

                DialogInteractions.OpenFileBrowser.RegisterHandler(interaction =>
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

                    if (dialog.ShowDialog(this) is true)
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

                DialogInteractions.OpenFolderBrowser.RegisterHandler(interaction =>
                {
                    var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

                    if (dialog.ShowDialog(this) is true)
                    {
                        var directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                        interaction.SetOutput(directoryInfo);
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                }).DisposeWith(d);

                MessageInteractions.ShowWarning.RegisterHandler(interaction =>
                {
                    var message = ExceptionHelper.GetAllMessages(interaction.Input);

                    MessageHelper.Warning(message, this);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                MessageInteractions.ShowInformation.RegisterHandler(interaction =>
                {
                    var message = interaction.Input;

                    MessageHelper.Information(message, this);

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

                ViewModel.PlayCompletionSound.RegisterHandler(interaction =>
                {
                    SystemSounds.Asterisk.Play();
                    interaction.SetOutput(Unit.Default);
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
