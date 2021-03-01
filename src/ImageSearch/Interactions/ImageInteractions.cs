using System;
using System.Reactive;
using ReactiveUI;

namespace ImageSearch.Interactions
{
    public static class ImageInteractions
    {
        public static Interaction<Uri, Unit> SearchForSimilar { get; } = new Interaction<Uri, Unit>();
    }
}
