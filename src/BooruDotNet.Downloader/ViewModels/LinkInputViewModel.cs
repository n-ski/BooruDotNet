using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using BooruDotNet.Downloader.Helpers;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class LinkInputViewModel : ReactiveObject
    {
        private string _inputText;
        private readonly ObservableAsPropertyHelper<IEnumerable<string>> _links;

        public LinkInputViewModel()
        {
            _links = this
                .WhenAnyValue(x => x.InputText)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(text => from line in text?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                where Uri.IsWellFormedUriString(line, UriKind.Absolute)
                                select line)
                .ObserveOn(RxApp.MainThreadScheduler) // Required due to Throttle() call
                .ToProperty(this, x => x.Links); // Doesn't work when shown as dialog.

#if DEBUG
            this.WhenAnyValue(x => x.Links).Subscribe(_ => Logger.Debug("you should see this"));
#endif

            Ok = ReactiveCommand.Create(
                ReactiveHelper.DoNothing,
                this.WhenAnyValue(x => x.Links, links => links?.Any() == true));
        }

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public IEnumerable<string> Links => _links.Value;

        // Dummy command for controlling executability.
        // Should probably stay in the view but this works for now.
        public ReactiveCommand<Unit, Unit> Ok { get; }
    }
}
