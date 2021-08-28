using System;
using System.Reactive;
using ReactiveUI;

namespace BooruDownloader.Interactions
{
    public static class MessageInteractions
    {
        public static Interaction<string, Unit> ShowInformation { get; } = new Interaction<string, Unit>();
        public static Interaction<Exception, Unit> ShowWarning { get; } = new Interaction<Exception, Unit>();
    }
}
