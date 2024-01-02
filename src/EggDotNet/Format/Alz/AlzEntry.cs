using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzEntry : IEggFileEntry
	{
		public FileHeader FileHeader { get; private set; }

		public int Id { get; private set; }
		public string Name => FileHeader.Name;
		public long Position => FileHeader.StartPosition;

		public long UncompressedSize => FileHeader.UncompressedSize;

		public long CompressedSize => FileHeader.CompressedSize;

		public uint Crc32 => FileHeader.Crc32;

		public CompressionMethod CompressionMethod => FileHeader.CompressionMethod;

		public bool IsEncrypted => false;

		public long ExternalAttributes => 0;

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		public DateTime? LastWriteTime => FileHeader.LastWriteTime;

		public string? Comment => string.Empty; //TODO
#else
		public DateTime LastWriteTime => FileHeader.LastWriteTime;

		public string Comment => string.Empty; //TODO
#endif
		public static List<AlzEntry> ParseEntries(Stream stream)
		{
			var entries = new List<AlzEntry>();

			while (stream.ReadInt(out int nextHeader))
			{
				var entry = new AlzEntry();

				if (nextHeader == FileHeader.ALZ_FILE_HEADER_START_MAGIC)
				{
					entry.FileHeader = FileHeader.Parse(stream);
					entry.Id = entries.Count;
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
				}
				else if (nextHeader == FileHeader.ALZ_FILE_HEADER_END_MAGIC)
				{
					break;
				}

				entries.Add(entry);
			}
			return entries;
		}
	}
}
