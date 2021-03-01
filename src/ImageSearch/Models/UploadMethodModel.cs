using ReactiveUI;
using Validation;

namespace ImageSearch.Models
{
    public class UploadMethodModel
    {
        public UploadMethodModel(UploadMethod uploadMethod, string name, ReactiveObject viewModel)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            Method = uploadMethod;
            Name = name;
            ViewModel = Requires.NotNull(viewModel, nameof(viewModel));
        }

        public UploadMethod Method { get; }
        public string Name { get; }
        public ReactiveObject ViewModel { get; }
    }
}
