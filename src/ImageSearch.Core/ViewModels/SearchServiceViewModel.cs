using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using ImageSearch.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class SearchServiceViewModel : ReactiveObject, IFileAndUriSearchService
    {
        private readonly IFileAndUriSearchService _service;
        private const float _defaultIconSize = 16;

        public SearchServiceViewModel(IFileAndUriSearchService service, string name, string iconResourceName)
        {
            _service = Requires.NotNull(service, nameof(service));
            Name = name;

            BitmapHelper.LoadBitmapAsync(iconResourceName, typeof(SearchServiceViewModel).Assembly, _defaultIconSize, _defaultIconSize)
                .ToObservable()
                .ToPropertyEx(this, x => x.Icon, scheduler: RxApp.MainThreadScheduler);
        }

        public string Name { get; }
        public IBitmap? Icon { [ObservableAsProperty] get; }
        public long FileSizeLimit => _service.FileSizeLimit;

        public Task<IEnumerable<IResult>> SearchAsync(FileStream fileStream, CancellationToken cancellationToken = default)
        {
            return _service.SearchAsync(fileStream, cancellationToken);
        }

        public Task<IEnumerable<IResult>> SearchAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return _service.SearchAsync(uri, cancellationToken);
        }
    }
}
