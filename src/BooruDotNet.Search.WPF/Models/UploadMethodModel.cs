using ReactiveUI;
using Validation;

namespace BooruDotNet.Search.WPF.Models
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
