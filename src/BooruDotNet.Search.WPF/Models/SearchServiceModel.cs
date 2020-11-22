using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using BooruDotNet.Search.Results;
using BooruDotNet.Search.Services;
using Validation;

namespace BooruDotNet.Search.WPF.Models
{
    public class SearchServiceModel : ISearchByUriAndFile
    {
        private readonly ISearchByUriAndFile _service;

        public SearchServiceModel(ISearchByUriAndFile service, string name, ImageSource icon)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            _service = Requires.NotNull(service, nameof(service));
            Name = name;
            Icon = Requires.NotNull(icon, nameof(icon));
        }

        public string Name { get; }
        public ImageSource Icon { get; }

        #region ISearchByUriAndFile implementation

        public Task<IEnumerable<IResult>> SearchByAsync(Uri uri, CancellationToken cancellationToken = default) =>
            _service.SearchByAsync(uri, cancellationToken);

        public Task<IEnumerable<IResult>> SearchByAsync(FileStream fileStream, CancellationToken cancellationToken = default) =>
            _service.SearchByAsync(fileStream, cancellationToken);

        #endregion
    }
}
