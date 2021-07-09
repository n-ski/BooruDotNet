using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class FileUploadViewModel : UploadViewModelBase
    {
        public FileUploadViewModel()
        {
            SelectFile = ReactiveCommand.CreateFromObservable(() => ShowFileSelection.Handle(Unit.Default));

            SelectFile.WhereNotNull().ToPropertyEx(this, x => x.FileToUpload);

            // Perform search when file is selected.
            this.WhenAnyValue(x => x.FileToUpload)
                .WhereNotNull()
                .Select(_ => Unit.Default)
                .InvokeCommand(this, x => x.Search);
        }

        public override UploadMethod UploadMethod => UploadMethod.File;

        public extern FileInfo? FileToUpload { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, FileInfo?> SelectFile { get; }

        public Interaction<Unit, FileInfo?> ShowFileSelection { get; } = new Interaction<Unit, FileInfo?>();
    }
}
