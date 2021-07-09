using System;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class UriUploadViewModel : UploadViewModelBase
    {
        public UriUploadViewModel()
        {
        }

        public override UploadMethod UploadMethod => UploadMethod.Uri;

        [Reactive]
        public Uri? FileUri { get; set; }
    }
}
