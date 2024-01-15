# EggDotNet ![pipeline badge](https://github.com/akolman/EggDotNet/actions/workflows/ci.yml/badge.svg)

## A decompression library for the Egg file format.
Supports both ALZ and EGG formats, with limitations (see wiki) as well as encrypted and split archives.

### What is Egg?
The EGG file format is an archive/file compression format developed by [ESTSoft](https://en.wikipedia.org/wiki/ESTsoft).  It is similar to the ZIP format, but supported certain features (e.g. Unicode filenames, split volumes) earlier than ZIP.  It is still in use today, but unless you live in South Korea you probably haven't heard of it.  ALZ/EGG files are authored using [ALZip](https://en.wikipedia.org/wiki/ALZip) on Windows.

## Usage

### Open an EGG archive from a `Stream`

```
using var inputArchiveStream = new FileStream("archive.egg");
using var archive = new EggArchive(inputArchiveStream);
using var firstEntryStream = archive.Entries.First().Open();
```

### Extract an EGG file to a directory
```
EggFile.ExtractToDirectory("archive.egg", "me/output");
```

Pure, safe C# and completely managed with no dependencies.  Compatible with netstandard2.0 and netstandard2.1.

### Install using NuGet
```
Install-Package EggDotNet
```