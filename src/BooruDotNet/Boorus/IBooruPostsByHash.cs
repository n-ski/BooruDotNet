using System.Threading.Tasks;
using BooruDotNet.Posts;

namespace BooruDotNet.Boorus
{
    public interface IBooruPostsByHash
    {
        Task<IPost> GetPostAsync(string hash);
    }
}
