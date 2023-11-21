
# EggDotNet
## An unarchiving library for the Egg file format.

Not intended for production use at this time as testing is currently limited.

### What is Egg?
The [EGG file format](https://en.wikipedia.org/wiki/EGG_(file_format)) is an archive/compression file format popular in South Korea, developed by [ESTSoft](https://en.wikipedia.org/wiki/ESTsoft). 

### Support checklist
- [x] EGG format
- [ ] ALZ format
- [x] Deflate compression
- [x] BZip2 compression
- [ ] AZO compression
- [ ] LZMA compression
- [x] Split archives
- [ ] Solid archives
- [ ] Encrypted headers
- [x] Encrypted files
- [x] File comments    

## Usage

### Open an EGG archive from a `Stream`

```
using var inputArchiveStream = new FileStream("archive.egg");
using var archive = new EggArchive(inputArchiveStream);
using var firstEntryStream = archive.Entries.First().Open();
```

Pure C# and completely managed with no dependencies.  Compatible with netstandard2.0 and netstandard2.1.
