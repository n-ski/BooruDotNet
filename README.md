A bunch of personal projects for coding practice.

This project was initially inspired by [BooruSharp](https://github.com/Xwilarg/BooruSharp).

## Library support

- Booru APIs
  - Danbooru: post by ID+MD5, tag by name
  - Gelbooru: post by ID+MD5, tag by name
  - Konachan: post by ID+MD5, tag by name
  - Sankaku Complex: post by ID
  - Yande.re: post by ID+MD5, tag by name
- Parse URL into a booru post (see above for supported sites)
- Reverse image search (with file or URL, see below for supported services)
- Create names for posts (post hash or Danbooru-style format)

## Application support

- BooruDownloader: download files from these sites
  - Danbooru
  - Gelbooru
  - Konachan
  - Sankaku Complex
  - Yande.re
- ImageSearch: reverse image search using one of these services
  - IQDB (multi-service)
  - Danbooru
  - Gelbooru

## TODO

- CLI application for automated reverse image search
- XML documentation
- NuGet packages for libraries
- Binaries for applications
- Proper todo/how-to/wiki ???
- Russian localization (exception messages, application UI)
- Make code less garbage (right now there's a lot of copypasta involved when implementing new boorus).