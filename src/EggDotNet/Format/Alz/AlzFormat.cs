using EggDotNet.Compression;
using EggDotNet.SpecialStreams;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzFormat : IEggFileFormat
	{
		private List<AlzVolume> _volumes;
		private List<AlzEntry> _entriesCache;
		private bool disposedValue;

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);
			var eggEntry = (AlzEntry)entry.entry;

			IStreamCompressionProvider streamProvider;
			if (eggEntry.CompressionMethod == CompressionMethod.Deflate)
			{
				streamProvider = new DeflateCompressionProvider();
			}
			else
			{
				streamProvider = new StoreCompressionProvider();
			}

            return streamProvider.GetDecompressStream(subSt);
		}

		public void ParseHeaders(Stream stream, bool ownStream)
		{
			var initialVolume = AlzVolume.Parse(stream, ownStream);
			_volumes = new List<AlzVolume>() { initialVolume };
		}

#pragma warning disable CA1859
		private Stream PrepareStream()
		{
			return new FakeDisposingStream(_volumes.Single().GetStream());
		}

		public List<EggArchiveEntry> Scan(EggArchive archive)
		{
			using (var st = PrepareStream())
			{
				_entriesCache = AlzEntry.ParseEntries(st);

				var ret = new List<EggArchiveEntry>();
				foreach (var entry in _entriesCache)
				{
					ret.Add(new EggArchiveEntry(entry, archive));
				}
				return ret;
			}
		}

		private void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_volumes != null)
					{
						foreach (var volume in _volumes)
						{
							volume.Dispose();
						}
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			System.GC.SuppressFinalize(this);
		}
	}
}
