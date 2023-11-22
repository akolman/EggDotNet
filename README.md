# EggDotNet
## An unarchiving library for the Egg file format.

Not intended for production use at this time as functionality is incomplete and testing is currently limited.

### What is Egg?
The [EGG file format](https://en.wikipedia.org/wiki/EGG_(file_format)) is an archive/compression format developed by [ESTSoft](https://en.wikipedia.org/wiki/ESTsoft).  Unless you live in South Korea you probably haven't heard of it.

### Support checklist
- [x] EGG format
- [x] ALZ format (*with limitations*)
- [x] Deflate compression
- [x] BZip2 compression
- [ ] AZO compression (see [issue #2](https://github.com/akolman/EggDotNet/issues/2))
- [ ] LZMA compression (see [issue #4](https://github.com/akolman/EggDotNet/issues/4))
- [x] Split archives
- [ ] Solid archives (see [issue #3](https://github.com/akolman/EggDotNet/issues/3))
- [ ] Encrypted headers
- [x] Encrypted files
- [x] Non-UTF8 filenames (e.g. shiftJIS)
- [ ] Encrypted filenames
- [x] Windows filesystem attributes
- [ ] POSIX filesystem attributes (see [issue #5](https://github.com/akolman/EggDotNet/issues/5))
- [x] File comments    

## Usage

### Open an EGG archive from a `Stream`

```
using var inputArchiveStream = new FileStream("archive.egg");
using var archive = new EggArchive(inputArchiveStream);
using var firstEntryStream = archive.Entries.First().Open();
```

Pure C# and completely managed with no dependencies.  Compatible with netstandard2.0 and netstandard2.1.
