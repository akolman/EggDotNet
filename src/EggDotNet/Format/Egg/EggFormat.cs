﻿using EggDotNet.Compression;
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
		private ICollection<EggVolume>? _volumes;
		private ICollection<EggEntry>? _entriesCache;
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

		private Stream GetDecompressionStream(Stream stream, EggEntry entry)
		{
			IStreamCompressionProvider? compressor = null;
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
				case CompressionMethod.Azo:
					throw new NotImplementedException("AZO not implemented");
				case CompressionMethod.Lzma:
					compressor = new LzmaCompressionProvider();
					break;
				default:
					break;
			}

			if (compressor == null)
			{
				throw new System.Exception("Compression not supported");
			}

			return compressor.GetDecompressStream(stream);
		}

		public Stream GetStreamForEntry(EggArchiveEntry entry)
		{
			var st = PrepareStream();
			Stream subSt = new SubStream(st, entry.PositionInStream, entry.PositionInStream + entry.CompressedLength);
			var eggEntry = _entriesCache.Single(e => e.Id == entry.Id);

			if (eggEntry.EncryptHeader != null)
			{
				while (true)
				{
					var pw = DefaultStreamCallbacks.GetPasswordCallback().Invoke();
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