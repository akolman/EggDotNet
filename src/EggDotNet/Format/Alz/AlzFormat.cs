using EggDotNet.Compression;
using EggDotNet.Format.Egg;
using EggDotNet.SpecialStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzFormat : IEggFileFormat
	{
		private List<AlzVolume>? _volumes;
		private List<AlzEntry>? _entriesCache;

		public void Dispose()
		{

		}

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);
			var eggEntry = _entriesCache.Single(e => e.Id == entry.Id);

			return new DeflateCompressionProvider().GetDecompressStream(subSt);
		}

		public void ParseHeaders(Stream stream, bool ownStream)
		{
			var initialVolume = AlzVolume.Parse(stream);
			_volumes = new List<AlzVolume>() { initialVolume };
		}

#pragma warning disable CA1859
		private Stream PrepareStream()
		{
			var st = new FakeDisposingStream(_volumes.Single().GetStream());

			return st;
		}

		public List<EggArchiveEntry> Scan(EggArchive archive)
		{
			using var st = PrepareStream();

			_entriesCache = AlzEntry.ParseEntries(st);

			var ret = new List<EggArchiveEntry>();
			foreach (var entry in _entriesCache)
			{
				ret.Add(new EggArchiveEntry(this, archive)
				{
					FullName = entry.Name,
					PositionInStream = entry.Position,
					CompressedLength = entry.CompressedSize,
					UncompressedLength = entry.UncompressedSize,
					//LastWriteTime = entry.LastModifiedTime,
					//Comment = entry.Comment,
					//IsEncrypted = entry.EncryptHeader != null,
					Archive = archive,
					Id = entry.Id,
					//Crc32 = entry.Crc
				});
			}
			return ret;
		}
	}
}
