using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easy.Common;

namespace BooruDotNet.Caches
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
                Ensure.NotNull(equalityComparer, nameof(equalityComparer)));
        }

        protected ConcurrentDictionary<TKey, Task<TValue>> Cache { get; }
    }
}
