using System.Threading.Tasks;
using BooruDotNet.Tags;

namespace BooruDotNet.Boorus
{
    public interface IBooruTagByName
    {
        Task<ITag> GetTagAsync(string tagName);
    }
}
