using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class FileUploadViewModel : UploadViewModelBase
    {
        public FileUploadViewModel()
        {
            SelectFile = ReactiveCommand.CreateFromObservable(() => ShowFileSelection.Handle(Unit.Default));

            SelectFile.WhereNotNull().BindTo(this, x => x.FileToUpload);

            Search = ReactiveCommand.CreateFromObservable(
                (SearchServiceViewModel service) => Observable.StartAsync(ct => SearchImpl(service, ct)).TakeUntil(CancelSearch ?? Observable.Empty<Unit>()),
                this.WhenAnyValue(x => x.FileToUpload, selector: file => file is object));
        }

        #region Properties

        public override UploadInputKind UploadInputKind => UploadInputKind.File;

        [Reactive]
        public FileInfo? FileToUpload { get; set; }

        public ReactiveCommand<Unit, FileInfo?> SelectFile { get; }

        #endregion

        #region Commands

        public override ReactiveCommand<SearchServiceViewModel, IReadOnlyCollection<SearchResultViewModel>> Search { get; }

        #endregion

        #region Interactions

        public Interaction<Unit, FileInfo?> ShowFileSelection { get; } = new Interaction<Unit, FileInfo?>();

        #endregion

        #region Command implementations

        private async Task<IReadOnlyCollection<SearchResultViewModel>> SearchImpl(SearchServiceViewModel service, CancellationToken ct)
        {
            CurrentStatusObserver.OnNext("Please wait\u2026");

            using FileStream fileStream = FileToUpload!.OpenRead();

            var results = await service.SearchAsync(fileStream, ct);

            return results.Select(x => new SearchResultViewModel(x)).ToArray();
        }

        #endregion
    }
}
