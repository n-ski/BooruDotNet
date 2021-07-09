using System.Reactive;
using ImageSearch.Helpers;
using ReactiveUI;

namespace ImageSearch.ViewModels
{
    public abstract class UploadViewModelBase : ReactiveObject
    {
        protected UploadViewModelBase()
        {
            // Do nothing. Parent will subscribe to this and react accordingly.
            Search = ReactiveCommand.Create(MethodHelper.DoNothing);
        }

        public abstract UploadMethod UploadMethod { get; }

        public ReactiveCommand<Unit, Unit> Search { get; }
    }
}
