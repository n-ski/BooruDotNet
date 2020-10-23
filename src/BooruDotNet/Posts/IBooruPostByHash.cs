using System.Threading;
using System.Threading.Tasks;

namespace BooruDotNet.Posts
{
    public interface IBooruPostByHash
    {
        Task<IPost> GetPostAsync(string hash, CancellationToken cancellationToken = default);
    }
}
