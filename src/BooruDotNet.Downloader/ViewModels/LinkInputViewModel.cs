using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace BooruDotNet.Downloader.ViewModels
{
    public class LinkInputViewModel : ReactiveObject
    {
        private string _inputText;
        private readonly ObservableAsPropertyHelper<IEnumerable<string>> _links;
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        public LinkInputViewModel()
        {
            _links = this
                .WhenAnyValue(x => x.InputText)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(text => from line in text?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                where Uri.IsWellFormedUriString(line, UriKind.Absolute)
                                select line)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Links);

            _isValid = this.WhenAnyValue(x => x.Links, links => links?.Any() == true)
                .ToProperty(this, x => x.IsValid);
        }

        public string InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public IEnumerable<string> Links => _links.Value;

        public bool IsValid => _isValid.Value;
    }
}
