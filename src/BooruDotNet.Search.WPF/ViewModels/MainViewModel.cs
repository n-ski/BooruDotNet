﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BooruDotNet.Search.WPF.Extensions;
using BooruDotNet.Search.WPF.Helpers;
using BooruDotNet.Search.WPF.Interactions;
using BooruDotNet.Search.WPF.Models;
using GongSolutions.Wpf.DragDrop;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.ViewModels
{
    public class MainViewModel : ReactiveObject, IDropTarget
    {
        private const double _bestMatchThreshold = 0.85;
        private SearchServiceModel _selectedService;
        private UploadMethodModel _selectedUploadMethod;
        private readonly UriUploadViewModel _uriUploadViewModel;
        private readonly FileUploadViewModel _fileUploadViewModel;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResults;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsBest;
        private readonly ObservableAsPropertyHelper<IEnumerable<ResultViewModel>> _searchResultsOther;
        private readonly ObservableAsPropertyHelper<bool> _hasBestResults;
        private readonly ObservableAsPropertyHelper<bool> _hasOtherResults;
        private readonly ObservableAsPropertyHelper<bool> _isSearching;
        // TODO: turn this into a setting.
        private const bool _searchImmeaditelyAfterDrop = true;

        public MainViewModel()
        {
            _uriUploadViewModel = new UriUploadViewModel();
            _fileUploadViewModel = new FileUploadViewModel();

            SearchServices = new[]
            {
                WPF.SearchServices.Danbooru,
                WPF.SearchServices.DanbooruIqdb,
                WPF.SearchServices.GelbooruIqdb,
            };

            UploadMethods = new[]
            {
                new UploadMethodModel(UploadMethod.Uri, "URL", _uriUploadViewModel),
                new UploadMethodModel(UploadMethod.File, "File", _fileUploadViewModel),
            };
            SetUploadMethod(UploadMethod.Uri);

            SearchCommand = ReactiveCommand.CreateFromObservable(
                () => Observable.StartAsync(LoadResultsAsync).TakeUntil(CancelSearchCommand),
                this.WhenAnyValue(
                    x => x.SelectedUploadMethod,
                    x => x._uriUploadViewModel.ImageUri,
                    x => x._fileUploadViewModel.FileInfo,
                    (model, uri, fileInfo) =>
                    {
                        return model?.Method switch
                        {
                            UploadMethod.Uri => UriHelper.IsValid(uri),
                            UploadMethod.File => fileInfo?.Exists ?? false,
                            _ => false,
                        };
                    }));

            SearchCommand.ThrownExceptions.Subscribe(
                async ex => await MessageInteractions.Exception.Handle(ex));

            CancelSearchCommand = ReactiveCommand.Create(
                CommandHelper.DoNothing,
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

            ImageInteractions.SearchForSimilar.RegisterHandler(interaction =>
            {
                SetUploadMethod(UploadMethod.Uri);
                _uriUploadViewModel.ImageUri = interaction.Input;

                // TODO: the subscription produced here needs to be disposed.
                Observable.Return(Unit.Default).InvokeCommand(this, x => x.SearchCommand);

                // Handle the interaction.
                interaction.SetOutput(Unit.Default);
            });
        }

        public SearchServiceModel SelectedService
        {
            get => _selectedService;
            set => this.RaiseAndSetIfChanged(ref _selectedService, value);
        }

        public UploadMethodModel SelectedUploadMethod
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
        public IEnumerable<SearchServiceModel> SearchServices { get; }
        public IEnumerable<UploadMethodModel> UploadMethods { get; }

        public ReactiveCommand<Unit, IEnumerable<ResultViewModel>> SearchCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelSearchCommand { get; }

        #region Drag and Drop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.TryGetDroppedFiles(out IEnumerable<string> files)
                && files.Any(FileHelper.IsFileValid))
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
                SetUploadMethod(UploadMethod.File);
                _fileUploadViewModel.FileInfo = new FileInfo(path);

                if (_searchImmeaditelyAfterDrop)
                {
                    // TODO: the subscription produced here needs to be disposed.
                    Observable.Return(Unit.Default).InvokeCommand(this, x => x.SearchCommand);
                }
            }
        }

        #endregion

        private async Task<IEnumerable<ResultViewModel>> LoadResultsAsync(CancellationToken cancellationToken)
        {
            // Task.Run(...) fixes the command blocking the UI.
            var task = Task
                .Run(async () =>
                {
                    switch (SelectedUploadMethod.Method)
                    {
                        case UploadMethod.Uri:
                            return await SelectedService.SearchByAsync(_uriUploadViewModel.ImageUri, cancellationToken);

                        case UploadMethod.File:
                            using (var fileStream = _fileUploadViewModel.FileInfo.OpenRead())
                            {
                                return await SelectedService.SearchByAsync(fileStream, cancellationToken);
                            }

                        default:
                            throw new InvalidOperationException();
                    }
                }, cancellationToken)
                .ConfigureAwait(false);
            var results = await task;
            return results.Select(r => new ResultViewModel(r));
        }

        private void SetUploadMethod(UploadMethod uploadMethod)
        {
            SelectedUploadMethod = UploadMethods.First(x => x.Method == uploadMethod);
        }
    }
}
