using System;
using System.Reactive.Linq;

namespace BooruDotNet.Search.WPF.Extensions
{
    internal static class ReactiveExtensions
    {
        internal static IObservable<T> SubscribeWithDispose<T>(this IObservable<T> source)
        {
            IDisposable subscription = source.Subscribe();
            return source.Finally(subscription.Dispose);
        }
    }
}
