using System.Reactive;
using ImageSearch.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class StatusViewModel : ReactiveObject
    {
        public StatusViewModel()
        {
            CancelOperation = ReactiveCommand.Create(
                MethodHelper.DoNothing,
                this.WhenAnyValue(x => x.IsActive));
        }

        [Reactive]
        public bool IsActive { get; set; }

        [Reactive]
        public string? StatusText { get; set; }

        public ReactiveCommand<Unit, Unit> CancelOperation { get; }
    }
}
