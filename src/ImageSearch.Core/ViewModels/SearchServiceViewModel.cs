using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using ReactiveUI;
using Splat;
using Validation;

namespace ImageSearch.ViewModels
{
    public class SearchServiceViewModel : ReactiveObject, IFileAndUriSearchService
    {
        private readonly IFileAndUriSearchService _service;

        public SearchServiceViewModel(IFileAndUriSearchService service, string name, IBitmap icon)
        {
            _service = Requires.NotNull(service, nameof(service));
            Name = name;
            Icon = icon;
        }

        public string Name { get; }
        public IBitmap Icon { get; }
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
