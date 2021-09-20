#nullable disable
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BooruDotNet.Helpers;
using GongSolutions.Wpf.DragDrop;
using ImageSearch.ViewModels;
using ImageSearch.WPF.Helpers;
using ReactiveUI;

namespace ImageSearch.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>, IDropTarget
    {
        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(SearchResultsScrollViewer, this);
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(SearchResultsScrollViewer, true);

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.BestSearchResultsViewModel, v => v.BestSearchResultsGroup.ViewModel)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.OtherSearchResultsViewModel, v => v.OtherSearchResultsGroup.ViewModel)
                    .DisposeWith(d);

                #region Status indicator

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel, v => v.BusyIndicator.BusyContent)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StatusViewModel.IsActive, v => v.BusyIndicator.IsBusy)
                    .DisposeWith(d);

                #endregion

                #region Upload methods

                this.OneWayBind(ViewModel, vm => vm.UploadMethods, v => v.UploadMethodsComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedUploadMethod, v => v.UploadMethodsComboBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.SelectedUploadMethod, v => v.UploadMethodHost.ViewModel)
                    .DisposeWith(d);

                #endregion

                #region Search services

                this.OneWayBind(ViewModel, vm => vm.SearchServices, v => v.SearchServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedSearchService, v => v.SearchServicesComboBox.SelectedItem)
                    .DisposeWith(d);

                #endregion

                this.BindCommand(
                    ViewModel,
                    vm => vm.Search,
                    v => v.SearchButton,
                    this.WhenAnyValue(v => v.ViewModel.SelectedSearchService))
                    .DisposeWith(d);

                // Scroll to top after the search command is completed.
                this.WhenAnyObservable(x => x.ViewModel.Search)
                    .Subscribe(_ => SearchResultsScrollViewer.ScrollToTop())
                    .DisposeWith(d);

                #region Interactions

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

                this.BindInteraction(ViewModel, vm => vm.DisplaySearchError, interaction =>
                {
                    Exception exception = interaction.Input.InnerException ?? interaction.Input;

                    string message = string.Join(
                        Environment.NewLine,
                        $"Exception of type '{exception.GetType()}' has occurred with the following message:",
                        exception.Message);

                    MessageBox.Show(this, message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    interaction.SetOutput(Unit.Default);
                    return Observable.Return(Unit.Default);
                }).DisposeWith(d);

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
                FileInfo file = DropDataHelper.GetFirstDroppedFile(data);

                if (FileHelper.IsImageFile(file))
                {
                    dropInfo.Effects = DragDropEffects.Link;
                    return;
                }
            }

            dropInfo.Effects = DragDropEffects.None;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var data = (DataObject)dropInfo.Data;
            var files = data.GetFileDropList();
            var firstFile = new FileInfo(files[0]);

            ViewModel.SearchWithFile.Execute(firstFile).Subscribe();
        }
    }
}
