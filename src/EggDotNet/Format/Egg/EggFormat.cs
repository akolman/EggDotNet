using EggDotNet.Compression;
using EggDotNet.Encryption;
using EggDotNet.Exception;
using EggDotNet.SpecialStreams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EggDotNet.Format.Egg
{
#pragma warning disable CA1852

	internal class EggFormat : IEggFileFormat
	{
		private readonly Func<Stream, IEnumerable<Stream>>? _streamCallback;
		private readonly Func<string>? _pwCallback;
		private List<EggVolume>? _volumes;
		private List<EggEntry>? _entriesCache;
		private bool disposedValue;

		internal EggFormat(Func<Stream, IEnumerable<Stream>>? streamCallback, Func<string>? pwCallback)
		{
			_streamCallback = streamCallback;
			_pwCallback = pwCallback;
		}

		public void ParseHeaders(Stream stream, bool ownStream)
		{
			var initialVolume = EggVolume.Parse(stream, ownStream);
			_volumes = new List<EggVolume>() { initialVolume };

			if (initialVolume.IsSplit)
			{
				if (_streamCallback == null)
				{
					throw new InvalidOperationException("Stream callback not set");
				}

				var streams = _streamCallback.Invoke(stream);

				foreach (var extStream in streams)
				{
					extStream.Seek(4, SeekOrigin.Begin);
					var extVolume = EggVolume.Parse(extStream, true);
					if (extVolume.Header.HeaderId != initialVolume.Header.HeaderId)
					{
						_volumes.Add(extVolume);
					}
				}
			}
			
		}

		private Stream PrepareStream()
		{
			Stream? st = null;
			if (_volumes != null && _volumes.Count > 1)
			{
				var subStreams = new List<SubStream>(_volumes.Count);
				var curVol = _volumes.Single(v => v.Header.SplitHeader!.PreviousFileId == 0);
				var curSt = curVol.GetStream();
				//subStreams.Add(new SubStream(curSt, curSt.Position));
				subStreams.Add(new SubStream(curSt, curVol.Header.HeaderEndPosition));

				while (curVol.Header.SplitHeader!.NextFileId != 0)
				{
					curVol = _volumes.Single(v => v.Header.HeaderId == curVol.Header.SplitHeader.NextFileId);
					curSt = curVol.GetStream();
					subStreams.Add(new SubStream(curSt, curVol.Header.HeaderEndPosition));
				}

				st = new CollectiveStream(subStreams);
			}
			else
			{
				st = new FakeDisposingStream(_volumes.Single().GetStream());
			}

			return st;
		}

		public List<EggArchiveEntry> Scan(EggArchive archive)
		{
			using var st = PrepareStream();
			
			_entriesCache = EggEntry.Parse(st);

			var ret = new List<EggArchiveEntry>();
			foreach (var entry in _entriesCache)
			{
				ret.Add(new EggArchiveEntry(this, archive)
				{
					FullName = entry.Name,
					PositionInStream = entry.Position,
					CompressedLength = entry.CompressedSize,
					UncompressedLength = entry.UncompressedSize,
					LastWriteTime = entry.LastModifiedTime,
					Comment = entry.Comment,
					IsEncrypted = entry.EncryptHeader != null,
					Archive = archive,
					Id = entry.Id,
					Crc32 = entry.Crc
				});
			}
			return ret;
		}

		private static Stream GetDecompressionStream(Stream stream, EggEntry entry)
		{
			IStreamCompressionProvider? compressor;
			compressor = entry.CompressionMethod switch
			{
				CompressionMethod.Store => new StoreCompressionProvider(),
				CompressionMethod.Deflate => new DeflateCompressionProvider(),
				CompressionMethod.Bzip2 => new BZip2CompressionProvider(),
				CompressionMethod.Azo => throw new NotImplementedException("AZO not implemented"),
				CompressionMethod.Lzma => new LzmaCompressionProvider(),
				_ => throw new UnknownCompressionEggception(),
			};
			return compressor.GetDecompressStream(stream);
		}

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);
			var eggEntry = _entriesCache.Single(e => e.Id == entry.Id);

			if (eggEntry.EncryptHeader != null)
			{
				var pwCb = _pwCallback ?? DefaultStreamCallbacks.GetPasswordCallback();

				while (true)
				{
					var pw = pwCb.Invoke();
					if (eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.Standard)
					{
						throw new NotImplementedException("ZIP encryption not yet supported");
					}
					else
					{
						var width = eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.AES256 ? 256 : 128;
						var s = new AesStreamDecryptionProvider(width, eggEntry.EncryptHeader.AesHeader, eggEntry.EncryptHeader.AesFooter, pw);
						if (s.PasswordValid)
						{
							subSt = s.GetDecryptionStream(subSt);
							break;
						}
					}
				}
			}

			return GetDecompressionStream(subSt, eggEntry);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue && disposing)
			{
				if (_volumes != null)
				{
					foreach (var volume in _volumes)
					{
						volume.Dispose();
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
