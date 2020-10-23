using System.Threading.Tasks;
using BooruDotNet.Posts;

namespace BooruDotNet.Boorus
{
    public interface IBooruPosts
    {
        Task<IPost> GetPostAsync(int id);
    }
}
