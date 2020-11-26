using System;
using System.Reactive;
using ReactiveUI;

namespace BooruDotNet.Search.WPF.Interactions
{
    public static class ImageInteractions
    {
        public static Interaction<Uri, Unit> SearchForSimilar { get; } = new Interaction<Uri, Unit>();
    }
}
