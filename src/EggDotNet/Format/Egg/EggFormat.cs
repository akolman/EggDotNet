using EggDotNet.Compression;
using EggDotNet.Encryption;
using EggDotNet.Extensions;
using EggDotNet.SpecialStreams;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class EggFormat : IEggFileFormat
	{
		private ICollection<EggVolume> _volumes;
		private ICollection<EggEntry> _entriesCache;
		private bool disposedValue;

		internal EggFormat()
		{
		}

		public void ParseHeaders(Stream stream, bool ownStream, Func<Stream, IEnumerable<Stream>>? streamCallback)
		{
			var initialVolume = EggVolume.Parse(stream, ownStream);
			_volumes = new List<EggVolume>() { initialVolume };

			if (initialVolume.IsSplit)
			{
				if (streamCallback == null)
				{
					throw new InvalidOperationException("Stream callback not set");
				}

				var streams = streamCallback.Invoke(stream);

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
			if (_volumes.Count > 1)
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

		public List<EggArchiveEntry> Scan()
		{
			using var st = PrepareStream();
			
			_entriesCache = EggEntry.Parse(st);

			var ret = new List<EggArchiveEntry>();
			foreach (var entry in _entriesCache)
			{
				ret.Add(new EggArchiveEntry(this)
				{
					FullName = entry.Name,
					PositionInStream = entry.Position,
					CompressedLength = entry.CompressedSize,
					UncompressedLength = entry.UncompressedSize,
					LastWriteTime = entry.LastModifiedTime
				});
			}
			return ret;
		}

		private Stream GetDecompressionStream(Stream stream, EggEntry entry)
		{
			var compMethod = entry.CompressionMethod;
			if (compMethod == CompressionMethod.Store)
			{
				var store = new StoreCompression();
				return store.GetDecompressStream(stream);
			}
			else if (compMethod == CompressionMethod.Deflate)
			{
				var deflate = new DeflateCompression();
				return deflate.GetDecompressStream(stream);
			}
			else if (compMethod == CompressionMethod.Bzip2)
			{
				var bzip2 = new BZip2Compression();
				return bzip2.GetDecompressStream(stream);
			}
			else
			{
				throw new NotImplementedException("Compression method not implemented");
			}
		}

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);

			if (_entriesCache.First().EncryptHeader != null)
			{
				while (true)
				{
					var pw = DefaultStreamCallbacks.GetPasswordCallback().Invoke();
					var s = new Aes256Decryption(256, _entriesCache.First().EncryptHeader.AesHeader, _entriesCache.First().EncryptHeader.AesFooter, pw);
					if (s.PasswordValid)
					{
						subSt = s.GetDecryptionStream(subSt);
						break;
					}
				}
			}

			var decompst = GetDecompressionStream(subSt, _entriesCache.Where(e => e.Name == entry.Name).Single());




			return decompst;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					foreach(var volume in _volumes)
					{
						volume.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
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
