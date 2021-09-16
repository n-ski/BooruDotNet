using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class UriUploadViewModel : UploadViewModelBase
    {
        public UriUploadViewModel()
        {
            Search = ReactiveCommand.CreateFromObservable(
                (SearchServiceViewModel service) => Observable.StartAsync(ct => SearchImpl(service, ct)).TakeUntil(CancelSearch ?? Observable.Empty<Unit>()),
                this.WhenAnyValue(x => x.FileUri, selector: uri => uri?.IsAbsoluteUri is true));
        }

        #region Properties

        public override UploadMethod UploadMethod => UploadMethod.Uri;

        [Reactive]
        public Uri? FileUri { get; set; }

        #endregion

        #region Commands

        public override ReactiveCommand<SearchServiceViewModel, IReadOnlyCollection<SearchResultViewModel>> Search { get; }

        #endregion

        #region Command implementations

        private async Task<IReadOnlyCollection<SearchResultViewModel>> SearchImpl(SearchServiceViewModel service, CancellationToken ct)
        {
            CurrentStatusObserver.OnNext("Please wait\u2026");

            var results = await service.SearchAsync(FileUri!, ct);

            return results.Select(x => new SearchResultViewModel(x)).ToArray();
        }

        #endregion
    }
}
