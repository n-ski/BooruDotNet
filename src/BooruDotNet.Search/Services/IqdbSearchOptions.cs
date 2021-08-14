using System;
using BooruDotNet.Search.Attributes;

namespace BooruDotNet.Search.Services
{
    /// <summary>
    /// Specifies flags to customize searches with <see cref="IqdbService"/>.
    /// </summary>
    [Flags]
    public enum IqdbSearchOptions
    {
        /// <summary>
        /// Specifies that the default options will be used.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Specifies that search results from <see href="https://danbooru.donmai.us/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("1")]
        SearchDanbooru = 1 << 0,

        /// <summary>
        /// Specifies that search results from <see href="https://konachan.com/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("2")]
        SearchKonachan = 1 << 1,

        /// <summary>
        /// Specifies that search results from <see href="https://yande.re/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("3")]
        SearchYandere = 1 << 2,

        /// <summary>
        /// Specifies that search results from <see href="https://gelbooru.com/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("4")]
        SearchGelbooru = 1 << 3,

        /// <summary>
        /// Specifies that search results from <see href="https://chan.sankakucomplex.com/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("5")]
        SearchSankakuChannel = 1 << 4,

        /// <summary>
        /// Specifies that search results from <see href="http://e-shuushuu.net/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("6")]
        SearchEShuushuu = 1 << 5,

        /// <summary>
        /// Specifies that search results from <see href="http://zerochan.net/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("11")]
        SearchZerochan = 1 << 6,

        /// <summary>
        /// Specifies that search results from <see href="https://anime-pictures.net/"/> will be included.
        /// </summary>
        [IqdbImageBoardId("13")]
        SearchAnimePictures = 1 << 7,
    }
}
