using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BooruDotNet;
using BooruDotNet.Helpers;
using BooruDotNet.Reactive.Interactions;
using BooruDotNet.Search.Results;
using GongSolutions.Wpf.DragDrop;
using Humanizer;
using ImageSearch.Extensions;
using ImageSearch.Helpers;
using ImageSearch.Interactions;
using ImageSearch.Models;
using ReactiveUI;
using Validation;

namespace ImageSearch.ViewModels
{
    public class MainViewModel : ReactiveObject, IDropTarget
    {
        private const double _bestMatchThreshold = 0.85;
        private const double _thumbnailSize = 500.0;
        private SearchServiceModel _selectedService;
        private UploadViewModelBase _selectedUploadMethod;
        private readonly UriUploadViewModel _uriUploadViewModel;
        private readonly FileUploadViewModel _fileUploadViewModel;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResults;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsBest;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsOther;
        private readonly ObservableAsPropertyHelper<bool> _hasBestResults;
        private readonly ObservableAsPropertyHelper<bool> _hasOtherResults;
        private readonly ObservableAsPropertyHelper<bool> _isSearching;
        // TODO: turn these into settings.
        private const bool _searchImmeaditelyAfterDrop = true;
        private const bool _searchImmeaditelyAfterPaste = true;
        private const bool _compressImages = true;

        public MainViewModel()
        {
            _uriUploadViewModel = new UriUploadViewModel("URL", UploadMethod.Uri);
            _fileUploadViewModel = new FileUploadViewModel("File", UploadMethod.File);

            UploadMethods = new UploadViewModelBase[] { _uriUploadViewModel, _fileUploadViewModel, };
            SetUploadMethod(UploadMethod.Uri);

            SearchCommand = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(SearchCommandImpl).TakeUntil(CancelSearchCommand),
                this.WhenAnyValue(
                    x => x.SelectedUploadMethod,
                    x => x._uriUploadViewModel.ImageUri,
                    x => x._fileUploadViewModel.FileInfo,
                    (model, uri, fileInfo) =>
                    {
                        return model?.UploadMethod switch
                        {
                            UploadMethod.Uri => Helpers.UriHelper.IsValid(uri),
                            UploadMethod.File => fileInfo?.Exists ?? false,
                            _ => false,
                        };
                    }));

            SearchCommand.ThrownExceptions.Subscribe(
                async ex => await MessageInteractions.ShowWarning.Handle(ex));

            CancelSearchCommand = ReactiveCommand.Create(
                MethodHelper.DoNothing,
                SearchCommand.IsExecuting);

            _isSearching = SearchCommand.IsExecuting.ToProperty(this, x => x.IsSearching);

            _searchResults = SearchCommand.ToProperty(
                this,
                x => x.SearchResults,
                initialValue: Enumerable.Empty<ResultViewModel>(),
                scheduler: RxApp.MainThreadScheduler);

            static bool hasAnyItems<T>(IEnumerable<T> enumerable) => enumerable.Any();

            #region Best results

            _searchResultsBest = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(results => from result in results
                                   where result.SourceUri != null && result.Similarity > _bestMatchThreshold
                                   select result)
                .ToProperty(this, x => x.SearchResultsBestMatches);

            _hasBestResults = this
                .WhenAnyValue(x => x.SearchResultsBestMatches)
                .Select(hasAnyItems)
                .ToProperty(this, x => x.HasBestResults);

            #endregion

            #region Other results

            _searchResultsOther = this
                .WhenAnyValue(x => x.SearchResults)
                .Select(results => from result in results
                                   where result.SourceUri != null && result.Similarity <= _bestMatchThreshold
                                   select result)
                .ToProperty(this, x => x.SearchResultsOtherMatches);

            _hasOtherResults = this
                .WhenAnyValue(x => x.SearchResultsOtherMatches)
                .Select(hasAnyItems)
                .ToProperty(this, x => x.HasOtherResults);

            #endregion

            ImageInteractions.SearchWithUri.RegisterHandler(interaction =>
            {
                SearchWithUri(interaction.Input, _searchImmeaditelyAfterPaste);

                interaction.SetOutput(Unit.Default);
            });

            ImageInteractions.SearchWithFile.RegisterHandler(interaction =>
            {
                SearchWithFile(interaction.Input, _searchImmeaditelyAfterPaste);

                interaction.SetOutput(Unit.Default);
            });
        }

        public SearchServiceModel SelectedService
        {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }

        public UploadViewModelBase SelectedUploadMethod
        {
            get => _selectedUploadMethod;
            set => this.RaiseAndSetIfChanged(ref _selectedUploadMethod, value);
        }

        public IEnumerable<ResultViewModel> SearchResults => _searchResults.Value;
        public IEnumerable<ResultViewModel> SearchResultsBestMatches => _searchResultsBest.Value;
        public IEnumerable<ResultViewModel> SearchResultsOtherMatches => _searchResultsOther.Value;
        public bool HasBestResults => _hasBestResults.Value;
        public bool HasOtherResults => _hasOtherResults.Value;
        public bool IsSearching => _isSearching.Value;
        public IEnumerable<UploadViewModelBase> UploadMethods { get; }

        public ReactiveCommand<Unit, IEnumerable<ResultViewModel>> SearchCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelSearchCommand { get; }

        #region Drag and Drop

        public void DragOver(IDropInfo dropInfo)
        {
            if ((dropInfo.TryGetDroppedFiles(out IEnumerable<string> files) && files.Any(FileHelper.IsFileValid))
                || (dropInfo.TryGetDroppedText(out string text) && Uri.IsWellFormedUriString(text, UriKind.Absolute)))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TryGetDroppedFiles(out IEnumerable<string> files)
                && files.FirstOrDefault(FileHelper.IsFileValid) is string path)
            {
                SearchWithFile(new FileInfo(path), _searchImmeaditelyAfterDrop);
            }
            else if (dropInfo.TryGetDroppedText(out string text)
                && Uri.TryCreate(text, UriKind.Absolute, out Uri uri))
            {
                SearchWithUri(uri, _searchImmeaditelyAfterDrop);
            }
        }

        #endregion

        private async Task<IEnumerable<ResultViewModel>> SearchCommandImpl(CancellationToken cancellationToken)
        {
            IEnumerable<IResult> results;

            switch (SelectedUploadMethod.UploadMethod)
            {
                case UploadMethod.Uri:
                    results = await SelectedService.SearchByAsync(_uriUploadViewModel.ImageUri, cancellationToken);
                    break;

                case UploadMethod.File:
                {
                    FileStream imageStream;

                    if (_compressImages)
                    {
                        using var originalFileStream = _fileUploadViewModel.FileInfo.OpenRead();
                        imageStream = await Task.Run(() => CompressImage(originalFileStream));
                    }
                    else
                    {
                        imageStream = _fileUploadViewModel.FileInfo.OpenRead();
                    }

                    using (imageStream)
                    {
                        results = await SelectedService.SearchByAsync(imageStream, cancellationToken);
                    }

                    break;
                }

                default:
                    throw Assumes.NotReachable();
            }

            return results.Select(result => new ResultViewModel(result));
        }

        private void SearchWithFile(FileInfo file, bool execute)
        {
            SetUploadMethod(UploadMethod.File);
            _fileUploadViewModel.FileInfo = file;

            if (execute)
            {
                ExecuteSearch();
            }
        }

        private void SearchWithUri(Uri uri, bool execute)
        {
            SetUploadMethod(UploadMethod.Uri);
            _uriUploadViewModel.ImageUri = uri;

            if (execute)
            {
                ExecuteSearch();
            }
        }

        private void SetUploadMethod(UploadMethod uploadMethod)
        {
            SelectedUploadMethod = UploadMethods.First(x => x.UploadMethod == uploadMethod);
        }

        // Invoke the command like this to avoid application crash in the exception interaction handler.
        private IDisposable ExecuteSearch() => Observable.Return(Unit.Default).InvokeCommand(this, x => x.SearchCommand);

        private FileStream CompressImage(FileStream originalFileStream)
        {
            var originalImage = ImageHelper.CreateImageFromStream(originalFileStream);

            var scale = _thumbnailSize / Math.Max(originalImage.PixelWidth, originalImage.PixelHeight);

            var resizedImage = scale < 1.0 ? ImageHelper.ScaleImage(originalImage, scale) : originalImage;

            var originalImageName = Path.GetFileNameWithoutExtension(originalFileStream.Name);
            var thumbnailImagePath = Path.Combine(Path.GetTempPath(), $"thumb-{originalImageName}.png");
            var tempFileStream = File.Create(thumbnailImagePath, 0x1000, FileOptions.DeleteOnClose);

            ImageHelper.SaveImage(resizedImage, tempFileStream);

            if (tempFileStream.Length > originalFileStream.Length)
            {
                tempFileStream.Dispose();
                tempFileStream = originalFileStream;

                Logger.Debug(
                    "Compressed image was larger than the original, original image will be uploaded.",
                    this);
            }
            else
            {
                static string toHumanBytes(long length) => length.Bytes().Humanize("0.00");

                Logger.Debug(
                    $"Compressed image: {toHumanBytes(originalFileStream.Length)} -> {toHumanBytes(tempFileStream.Length)}",
                    this);
            }

            tempFileStream.Position = 0;
            return tempFileStream;
        }
    }
}
