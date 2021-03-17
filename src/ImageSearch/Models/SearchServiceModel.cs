using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using Validation;

namespace ImageSearch.Models
{
    public class SearchServiceModel : IFileAndUriSearchService
    {
        private readonly IFileAndUriSearchService _service;

        public SearchServiceModel(IFileAndUriSearchService service, string name, ImageSource icon)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            _service = Requires.NotNull(service, nameof(service));
            Name = name;
            Icon = Requires.NotNull(icon, nameof(icon));
        }

        public string Name { get; }
        public ImageSource Icon { get; }
        public long FileSizeLimit => _service.FileSizeLimit;

        #region IFileAndUriSearchService implementation

        public Task<IEnumerable<IResult>> SearchAsync(Uri uri, CancellationToken cancellationToken = default) =>
            _service.SearchAsync(uri, cancellationToken);

        public Task<IEnumerable<IResult>> SearchAsync(FileStream fileStream, CancellationToken cancellationToken = default) =>
            _service.SearchAsync(fileStream, cancellationToken);

        #endregion
    }
}
