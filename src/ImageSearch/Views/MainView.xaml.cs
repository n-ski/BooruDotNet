using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BooruDotNet.Helpers;
using BooruDotNet.Reactive.Interactions;
using BooruDotNet.Search.Services;
using Humanizer;
using ImageSearch.Helpers;
using ImageSearch.Interactions;
using ImageSearch.ViewModels;
using Microsoft.Win32;
using ReactiveUI;

namespace ImageSearch.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>
    {
        private string _initialDirectory;

        public MainView()
        {
            _initialDirectory = Environment.CurrentDirectory;

            InitializeComponent();
            ViewModel = new MainViewModel();

            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(MainContentGrid, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(MainContentGrid, ViewModel);

#if DEBUG
            Title += " (Debug)";
#endif

            this.WhenActivated(d =>
            {
                #region Results bindings

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SearchResultsBestMatches,
                    v => v.BestResultsControl.ItemsSource)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SearchResultsOtherMatches,
                    v => v.OtherResultsControl.ItemsSource)
                    .DisposeWith(d);

                #endregion

                #region Best results visibility

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasBestResults,
                    v => v.BestResultsHeader.Visibility)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasBestResults,
                    v => v.BestResultsControl.Visibility)
                    .DisposeWith(d);

                #endregion

                #region Other results visibility

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasOtherResults,
                    v => v.OtherResultsHeader.Visibility)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.HasOtherResults,
                    v => v.OtherResultsControl.Visibility)
                    .DisposeWith(d);

                #endregion

                // Scroll to top when the results change.
                ViewModel
                    .WhenAnyValue(vm => vm.SearchResults)
                    .Subscribe(_ => ResultsScrollViewer.ScrollToTop())
                    .DisposeWith(d);

                this.BindCommand(
                    ViewModel,
                    vm => vm.SearchCommand,
                    v => v.SearchButton)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.IsSearching,
                    v => v.SearchBusyIndicator.IsBusy)
                    .DisposeWith(d);

                this.Events().KeyDown
                    .Where(e => e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.V)
                    .Subscribe(_ =>
                    {
                        if (Clipboard.ContainsImage())
                        {
                            var image = Clipboard.GetImage();
                            var fileInfo = FileHelper.SaveTempImage(image);

                            ImageInteractions.SearchWithFile.Handle(fileInfo).Subscribe();
                        }
                        else if (Clipboard.ContainsFileDropList())
                        {
                            var files = Clipboard.GetFileDropList();
                            var filePath = files.Count > 0 ? files[0] : null;

                            if (FileHelper.IsFileValid(filePath))
                            {
                                var fileInfo = new FileInfo(files[0]);

                                if (fileInfo.Exists)
                                {
                                    ImageInteractions.SearchWithFile.Handle(fileInfo).Subscribe();
                                }
                            }
                        }
                        else if (Clipboard.ContainsText())
                        {
                            var text = Clipboard.GetText().Trim();

                            if (text.StartsWith(Uri.UriSchemeHttp) || text.StartsWith(Uri.UriSchemeHttps))
                            {
                                var uri = new Uri(text);

                                ImageInteractions.SearchWithUri.Handle(uri).Subscribe();
                            }
                        }
                    })
                    .DisposeWith(d);

                #region Search services ComboBox

                Observable.Return(SearchServices.Services)
                    .BindTo(this, v => v.ServicesComboBox.ItemsSource)
                    .DisposeWith(d);

                // One-way to source binding.
                this.WhenAnyValue(v => v.ServicesComboBox.SelectedItem)
                    .BindTo(this, v => v.ViewModel.SelectedService)
                    .DisposeWith(d);

                #endregion

                #region Upload method ComboBox

                this.OneWayBind(
                    ViewModel,
                    vm => vm.UploadMethods,
                    v => v.UploadMethodComboBox.ItemsSource)
                    .DisposeWith(d);

                // Has to be a two-way binding since the viewmodel can also set upload method.
                this.Bind(
                    ViewModel,
                    vm => vm.SelectedUploadMethod,
                    v => v.UploadMethodComboBox.SelectedItem)
                    .DisposeWith(d);

                this.OneWayBind(
                    ViewModel,
                    vm => vm.SelectedUploadMethod,
                    v => v.UploadMethodViewHost.ViewModel)
                    .DisposeWith(d);

                #endregion

                #region Interactions

                DialogInteractions.OpenFileBrowser.RegisterHandler(interaction =>
                {
                    if (!Directory.Exists(_initialDirectory))
                    {
                        _initialDirectory = Environment.CurrentDirectory;
                    }

                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = interaction.Input,
                        InitialDirectory = _initialDirectory,
                        Multiselect = false,
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        var fileInfo = new FileInfo(dialog.FileName);

                        _initialDirectory = fileInfo.DirectoryName;
                        interaction.SetOutput(fileInfo);
                    }
                    else
                    {
                        interaction.SetOutput(null);
                    }
                }).DisposeWith(d);

                MessageInteractions.ShowWarning.RegisterHandler(interaction =>
                {
                    Exception exception = interaction.Input.InnerException ?? interaction.Input;

                    string message;

                    if (exception is FileTooLargeException fileTooLargeEx
                        && fileTooLargeEx.MaximumFileSize.HasValue)
                    {
                        var maxSizeBytes = fileTooLargeEx.MaximumFileSize.Value.Bytes();
                        message = $"File exceeds maximum file size of {maxSizeBytes.Humanize("0.##")}.";
                    }
                    else
                    {
                        message = exception.Message;
                    }

                    MessageHelper.Warning(
                        string.Join(
                            Environment.NewLine,
                            "The following exception has occured:",
                            exception.GetType(),
                            message),
                        this);

                    interaction.SetOutput(Unit.Default);
                }).DisposeWith(d);

                #endregion
            });
        }

        // TODO: there's probably a better way to do this.
        private void CancelSearchButton_Initialized(object sender, EventArgs e)
        {
            var button = (Button)sender;

            // Ideally this better be disposed.
            button
                .Events().Click
                .Select(_ => Unit.Default)
                .InvokeCommand(this, v => v.ViewModel.CancelSearchCommand);
        }
    }
}
