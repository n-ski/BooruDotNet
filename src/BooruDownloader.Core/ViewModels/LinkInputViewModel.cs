using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BooruDownloader.ViewModels
{
    public class LinkInputViewModel : ReactiveObject
    {
        public LinkInputViewModel()
        {
            this.WhenAnyValue(x => x.InputText)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .DistinctUntilChanged()
                .WhereNotNull()
                .Select(text => from line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                                let trimmed = line.Trim()
                                where Uri.IsWellFormedUriString(trimmed, UriKind.Absolute)
                                select trimmed)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.Links);

            this.WhenAnyValue(x => x.Links, links => links?.Any() is true)
                .ToPropertyEx(this, x => x.IsValid);
        }

        [Reactive]
        public string? InputText { get; set; }

        public extern IEnumerable<string>? Links { [ObservableAsProperty] get; }

        public extern bool IsValid { [ObservableAsProperty] get; }
    }
}
