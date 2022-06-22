using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Tags;

public interface IBooruTagByName
{
    Task<ITag> GetTagAsync(string tagName, CancellationToken cancellationToken = default);
}
