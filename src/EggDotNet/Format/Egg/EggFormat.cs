using EggDotNet.Compression;
using EggDotNet.Encryption;
using EggDotNet.Exceptions;
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
		private readonly Func<Stream, IEnumerable<Stream>> _streamCallback;
		private readonly Func<string> _pwCallback;
		private readonly List<EggVolume> _volumes = new List<EggVolume>(8);
		private List<EggEntry> _entriesCache;
		private bool disposedValue;

		internal EggFormat(Func<Stream, IEnumerable<Stream>> streamCallback, Func<string> pwCallback)
		{
			_streamCallback = streamCallback;
			_pwCallback = pwCallback;
		}

		public void ParseHeaders(Stream stream, bool ownStream)
		{
			var initialVolume = EggVolume.Parse(stream, ownStream);
			_volumes.Add(initialVolume);

			if (initialVolume.IsSplit)
			{
				FetchAndParseSplitVolumes(stream);
			}
		}

		public List<EggArchiveEntry> Scan(EggArchive archive)
		{
			using (var st = PrepareStream())
			{
				_entriesCache = EggEntry.ParseEntries(st, archive);

				var ret = new List<EggArchiveEntry>(_entriesCache.Count);
				foreach (var entry in _entriesCache)
				{
					ret.Add(new EggArchiveEntry(entry, archive));
				}
				return ret;
			}
		}

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);
			var eggEntry = (EggEntry)entry.entry;
			if (eggEntry.EncryptHeader != null)
			{
				subSt = GetDecryptionStream(subSt, eggEntry);
			}

			return GetDecompressionStream(subSt, eggEntry);
		}

		private void FetchAndParseSplitVolumes(Stream stream)
		{
			if (_streamCallback == null)
			{
				throw new InvalidOperationException("Stream callback not set");
			}

			var initialVolume = _volumes.Single();

			var streams = _streamCallback.Invoke(stream);

			foreach (var extStream in streams)
			{
				extStream.Seek(4, SeekOrigin.Begin);
				var extVolume = EggVolume.Parse(extStream, true);
				if (extVolume.Header.HeaderId != initialVolume.Header.HeaderId
					&& !_volumes.Any(v => v.Header.HeaderId == extVolume.Header.HeaderId))
				{
					_volumes.Add(extVolume);
				}
			}
		}

		private Stream PrepareStream()
		{
			if (_volumes.Count == 0) throw new InvalidOperationException("Volume collection empty");

			if (_volumes.Count == 1)
			{
				return new FakeDisposingStream(_volumes.Single().GetStream());
			}
			else
			{
				return PrepareSplitStream();
			}
		}

		private CollectiveStream PrepareSplitStream()
		{
			var subStreams = new List<SubStream>(_volumes.Count);
			var curVol = _volumes.Single(v => v.Header.SplitHeader.PreviousFileId == 0);
			var curSt = curVol.GetStream();
			subStreams.Add(new SubStream(curSt, curVol.Header.HeaderEndPosition));

			while (curVol.Header.SplitHeader.NextFileId != 0)
			{
				curVol = _volumes.Single(v => v.Header.HeaderId == curVol.Header.SplitHeader.NextFileId);
				curSt = curVol.GetStream();
				subStreams.Add(new SubStream(curSt, curVol.Header.HeaderEndPosition));
			}

			return new CollectiveStream(subStreams);
		}

		private Stream GetDecryptionStream(Stream subSt, EggEntry eggEntry)
		{
			var pwCb = _pwCallback ?? DefaultStreamCallbacks.GetPasswordCallback();

			while (true)
			{
				var pw = pwCb.Invoke();
				IStreamDecryptionProvider s;
				if (eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.Standard)
				{
					s = new ZipStreamDecryptionProvider(eggEntry.EncryptHeader.Param1, eggEntry.EncryptHeader.Param2, pw);
				}
				else if (eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.AES128 
					|| eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.AES256)
				{
					var width = eggEntry.EncryptHeader.EncryptionMethod == EncryptionMethod.AES256 ? 256 : 128;
					s = new AesStreamDecryptionProvider(width, eggEntry.EncryptHeader.Param1, eggEntry.EncryptHeader.Param2, pw);
				}
				else
				{
					throw new NotImplementedException("Encryption method not supported");
				}

				if (s.PasswordValid)
				{
					return s.GetDecryptionStream(subSt);
				}
			}
		}

		private static Stream GetDecompressionStream(Stream stream, EggEntry entry)
		{
			IStreamCompressionProvider compressor;
			switch (entry.CompressionMethod)
			{
				case CompressionMethod.Store:
					compressor = new StoreCompressionProvider();
					break;
				case CompressionMethod.Deflate:
					compressor = new DeflateCompressionProvider();
					break;
				case CompressionMethod.Bzip2:
					compressor = new BZip2CompressionProvider();
					break;
				case CompressionMethod.Lzma:
					compressor = new LzmaCompressionProvider(entry.CompressedSize, entry.UncompressedSize);
					break;
				case CompressionMethod.Azo:
					throw new NotImplementedException("AZO not implemented");
				default:
					throw new UnknownCompressionException((byte)entry.CompressionMethod);

			}

			return compressor.GetDecompressStream(stream);
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
