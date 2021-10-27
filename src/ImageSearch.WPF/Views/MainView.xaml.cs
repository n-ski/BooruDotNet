#nullable disable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BooruDotNet.Helpers;
using DynamicData;
using DynamicData.Binding;
using GongSolutions.Wpf.DragDrop;
using ImageSearch.Helpers;
using ImageSearch.ViewModels;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>, IDropTarget
    {
        private static readonly FileHelper.FileType[] _droppableFileTypes =
        {
            FileHelper.FileType.Bmp,
            FileHelper.FileType.Gif,
            FileHelper.FileType.Jpeg,
            FileHelper.FileType.Png,
        };

        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(App.HttpClient);

            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(QueueItemsListBox, this);
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(QueueItemsListBox, true);

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.QueuedItems, v => v.QueueItemsListBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedQueueItem, v => v.QueueItemsListBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SelectedQueueItem, v => v.SelectedQueueItemView.ViewModel)
                    .DisposeWith(d);

                // Scroll to top whenever the selected queue item is changed.
                this.WhenAnyValue(v => v.ViewModel.SelectedQueueItem)
                    .Subscribe(_ => SearchResultsScrollViewer.ScrollToTop())
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.AddFiles, v => v.AddFilesButton)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.ImageUri, v => v.ImageUriTextBox.Text)
                    .DisposeWith(d);

                ImageUriTextBox
                    .Events()
                    .KeyDown
                    .Where(args => args.Key is Key.Enter)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, v => v.ViewModel.AddUri)
                    .DisposeWith(d);

                // Clear the URL text box when a new item was added. Do it here instead of doing it in
                // the block above because we want to clear the box only when the command was executed.
                this.WhenAnyObservable(v => v.ViewModel.AddUri)
                    .Select(_ => string.Empty)
                    .BindTo(this, v => v.ImageUriTextBox.Text)
                    .DisposeWith(d);

                // Scroll the queue list to bottom whenever a new item is added.
                this.WhenAnyValue(v => v.ViewModel.QueuedItems, items => items.ToObservableChangeSet())
                    .Switch()
                    .OnItemAdded(item => QueueItemsListBox.ScrollIntoView(item))
                    .Subscribe()
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.ClearQueue, v => v.ClearQueueButton)
                    .DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.OpenSettings, v => v.OpenSettingsButton)
                    .DisposeWith(d);

                ViewSettings.Default.WhenAnyValue(x => x.ShowUploadTooltip)
                    .BindTo(this, v => v.UploadTipTextBlock.Visibility)
                    .DisposeWith(d);

                #region Search services

                this.OneWayBind(ViewModel, vm => vm.SearchServices, v => v.SearchServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedSearchService, v => v.SearchServicesComboBox.SelectedItem)
                    .DisposeWith(d);

                #endregion

                #region Interactions

                this.BindInteraction(ViewModel, vm => vm.SelectFilesInteraction, interaction =>
                {
                    var dialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "Images|*.gif;*.jpg;*.jpeg;*.png",
                        Multiselect = true,
                    };

                    FileInfo[] files = dialog.ShowDialog() is true
                        ? Array.ConvertAll(dialog.FileNames, path => new FileInfo(path))
                        : null;

                    interaction.SetOutput(files);
                    return Observable.Return(Unit.Default);
                }).DisposeWith(d);

                this.BindInteraction(ViewModel, vm => vm.OpenUriInteraction, interaction =>
                {
                    Uri uri = interaction.Input;
                    Debug.Assert(uri.IsAbsoluteUri);

                    using var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = uri.AbsoluteUri,
                        UseShellExecute = true,
                    });

                    interaction.SetOutput(Unit.Default);
                    return Observable.Return(Unit.Default);
                }).DisposeWith(d);

                this.BindInteraction(ViewModel, vm => vm.CopyUriInteraction, interaction =>
                {
                    Uri uri = interaction.Input;
                    Debug.Assert(uri.IsAbsoluteUri);

                    Clipboard.SetText(uri.AbsoluteUri);

                    interaction.SetOutput(Unit.Default);
                    return Observable.Return(Unit.Default);
                }).DisposeWith(d);

                this.BindInteraction(ViewModel, vm => vm.ShowSettingsDialog, interaction =>
                {
                    return Observable.Start(() =>
                    {
                        var dialog = new SettingsView
                        {
                            ViewModel = interaction.Input,
                            Owner = this,
                            Title = $"Settings - {Title}",
                        };

                        bool? result = dialog.ShowDialog();

                        interaction.SetOutput(result is true);
                    }, RxApp.MainThreadScheduler);
                });

                #endregion

                #region Pasted data handlers

                var pasteEvent = this.Events()
                    .KeyDown
                    .Where(args => args.KeyboardDevice.Modifiers is ModifierKeys.Control && args.Key is Key.V);

                // Pasted text.
                pasteEvent
                    .Where(_ => Clipboard.ContainsText())
                    .Select(_ => Clipboard.GetText())
                    .Select(text => Uri.TryCreate(text, UriKind.Absolute, out Uri uri) ? uri : null)
                    .WhereNotNull()
                    .InvokeCommand(this, v => v.ViewModel.SearchWithUri)
                    .DisposeWith(d);

                // Pasted file.
                pasteEvent
                    .Where(_ => Clipboard.ContainsFileDropList())
                    .Select(_ => Clipboard.GetFileDropList())
                    .Select(files => new FileInfo(files[0]))
                    .InvokeCommand(this, v => v.ViewModel.SearchWithFile)
                    .DisposeWith(d);

                // Pasted image.
                pasteEvent
                    .Where(_ => Clipboard.ContainsImage())
                    .Select(_ => Clipboard.GetImage())
                    .Catch((Exception ex) =>
                    {
                        Debug.WriteLine(ex, nameof(MainView));

                        MessageBoxResult result = MessageBox.Show(
                            this,
                            string.Join(
                                Environment.NewLine,
                                "An error has occured while reading the image from clipboard:",
                                ex.Message,
                                "Display stack trace?"),
                            "Error",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Error);

                        if (result is MessageBoxResult.Yes)
                        {
                            MessageBox.Show(
                                this,
                                ex.ToString(),
                                "Exception stack trace",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }

                        return Observable.Empty<BitmapSource>();
                    })
                    .Select(image =>
                    {
                        // Scale image down if one of its sides is larger than this.
                        const double threshold = 1000;
                        double scale = threshold / Math.Max(image.PixelWidth, image.PixelHeight);

                        if (scale >= 1.0)
                        {
                            return image;
                        }

                        return ImageHelper.ScaleImage(image, scale);
                    })
                    .Select(image =>
                    {
                        var fileInfo = new FileInfo(Path.GetTempFileName());

                        using (Stream stream = fileInfo.OpenWrite())
                        {
                            ImageHelper.SaveImage(image, stream);
                        }

                        return fileInfo;
                    })
                    .InvokeCommand(this, v => v.ViewModel.SearchWithFile)
                    .DisposeWith(d);

                #endregion
            });
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject data && data.ContainsFileDropList())
            {
                foreach (string filePath in data.GetFileDropList())
                {
                    if (FileHelper.IsAnyFileType(filePath, _droppableFileTypes))
                    {
                        dropInfo.Effects = DragDropEffects.Link;
                        return;
                    }
                }
            }

            dropInfo.Effects = DragDropEffects.None;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data)
                .GetFileDropList()
                .Cast<string>()
                .Where(filePath => FileHelper.IsAnyFileType(filePath, _droppableFileTypes))
                .Select(path => new FileInfo(path));

            ViewModel.SearchWithManyFiles.Execute(files).Subscribe();
        }
    }
}
