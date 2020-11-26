using System;
using System.Reactive;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Interactions
{
    public class MessageInteractions
    {
        public static Interaction<Exception, Unit> Exception { get; } = new Interaction<Exception, Unit>();
    }
}
