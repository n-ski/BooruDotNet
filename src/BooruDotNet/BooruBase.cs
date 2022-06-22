using System.Net.Http;
using Validation;

namespace BooruDotNet;

public abstract class BooruBase
{
    protected BooruBase(HttpClient httpClient)
    {
        HttpClient = Requires.NotNull(httpClient, nameof(httpClient));
    }

    protected HttpClient HttpClient { get; }
}
