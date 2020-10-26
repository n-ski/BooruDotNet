using System.Threading.Tasks;

namespace BooruDotNet.Posts
{
    public interface IBooruPostById
    {
        Task<IPost> GetPostAsync(int id);
    }
}
