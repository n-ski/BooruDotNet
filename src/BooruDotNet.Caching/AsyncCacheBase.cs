using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Validation;

namespace BooruDotNet.Caching
{
    // TODO: add cache flushing after a period of time.
    public abstract class AsyncCacheBase<TKey, TValue> where TKey : notnull
    {
        protected AsyncCacheBase()
        {
            Cache = new ConcurrentDictionary<TKey, Task<TValue>>();
        }

        protected AsyncCacheBase(IEqualityComparer<TKey> equalityComparer)
        {
            Cache = new ConcurrentDictionary<TKey, Task<TValue>>(
                Requires.NotNull(equalityComparer, nameof(equalityComparer)));
        }

        protected ConcurrentDictionary<TKey, Task<TValue>> Cache { get; }
    }
}
