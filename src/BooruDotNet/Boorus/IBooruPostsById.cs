using System.Threading.Tasks;
using BooruDotNet.Posts;

namespace BooruDotNet.Boorus
{
    public interface IBooruPostsById
    {
        Task<IPost> GetPostAsync(int id);
    }
}
