using System;
using System.Reactive;
using ReactiveUI;

namespace ImageSearch.Interactions
{
    public class MessageInteractions
    {
        public static Interaction<string, Unit> Warning { get; } = new Interaction<string, Unit>();
        public static Interaction<Exception, Unit> Exception { get; } = new Interaction<Exception, Unit>();
    }
}
