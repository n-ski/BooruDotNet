using BooruDotNet;
using BooruDotNet.Tags;
using ReactiveUI;
using Validation;

namespace BooruDownloader.ViewModels
{
    public class TagViewModel : ReactiveObject, ITag
    {
        private readonly ITag _tag;

        public TagViewModel(ITag tag)
        {
            _tag = Requires.NotNull(tag, nameof(tag));
        }

        public string Name => _tag.Name;

        public TagKind Kind => _tag.Kind;

        public int ID => _tag.ID;

        public int Count => _tag.Count;
    }
}
