using System.Threading.Tasks;

namespace BooruDotNet.Tags
{
    public interface IBooruTagByName
    {
        Task<ITag> GetTagAsync(string tagName);
    }
}
