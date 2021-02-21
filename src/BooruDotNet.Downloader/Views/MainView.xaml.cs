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

                this.BindCommand(ViewModel, vm => vm.DownloadPosts, v => v.DownloadButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.QueuedItems.Count,
                    v => v.DownloadButton.Content,
                    count => count > 0 ? $"Download {"file".ToQuantity(count)}" : "Download")
                    .DisposeWith(d);

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

                    MessageBox.Show(
                        message,
                        "Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                #region Add posts busy content

                this.OneWayBind(
                    ViewModel,
                    vm => vm.TotalPostsToAdd,
                    v => v.AddedItemsProgressLabel.Text,
                    total => $"Fetching posts from {"link".ToQuantity(total)}\u2026")
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.AddedPosts, v => v.AddedItemsProgressBar.Value)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.TotalPostsToAdd, v => v.AddedItemsProgressBar.Maximum)
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
            });
        }

        #region Add posts busy content

        internal StackPanel AddPostsBusyContent { get; }
        internal TextBlock AddedItemsProgressLabel { get; }
        internal ProgressBar AddedItemsProgressBar { get; }
        internal Button CancelAddButton { get; }

        #endregion
    }
}
