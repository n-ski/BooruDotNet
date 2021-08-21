using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImageSearch.ViewModels
{
    public class SearchResultGroupViewModel : ReactiveObject
    {
        public SearchResultGroupViewModel()
        {
            this.WhenAnyValue(x => x.SearchResults, results => results?.Any() is true)
                .ToPropertyEx(this, x => x.HasResults);
        }

        [Reactive]
        public IEnumerable<SearchResultViewModel>? SearchResults { get; set; }

        public extern bool HasResults { [ObservableAsProperty] get; }
    }
}
