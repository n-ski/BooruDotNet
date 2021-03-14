using ReactiveUI;
using Validation;

namespace ImageSearch.ViewModels
{
    public class UploadViewModelBase : ReactiveObject
    {
        public UploadViewModelBase(string name, UploadMethod uploadMethod)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));
            Name = name;

            Requires.Defined(uploadMethod, nameof(uploadMethod));
            UploadMethod = uploadMethod;
        }

        public string Name { get; }

        public UploadMethod UploadMethod { get; }
    }
}
