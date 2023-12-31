using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzEntry : IEggFileEntry
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public uint Crc32 { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		public bool IsEncrypted => false;

		public long ExternalAttributes => 0;

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		public DateTime? LastWriteTime { get; private set; }

		public string? Comment => throw new NotImplementedException();
#else
		public DateTime LastWriteTime { get; private set; }

		public string Comment => throw new NotImplementedException();
#endif
		public static List<AlzEntry> ParseEntries(Stream stream)
		{
			var entries = new List<AlzEntry>();

			while (stream.ReadInt(out int nextHeader))
			{
				var entry = new AlzEntry();

				if (nextHeader == FileHeader.ALZ_FILE_HEADER_START_MAGIC)
				{
					var fileHeader = FileHeader.Parse(stream);
					entry.Id = entries.Count;
					entry.CompressedSize = fileHeader.CompressedSize;
					entry.UncompressedSize = fileHeader.UncompressedSize;
					entry.CompressionMethod = fileHeader.CompressionMethod;
					entry.Name = fileHeader.Name;
					entry.Position = fileHeader.StartPosition;
					entry.Crc32 = fileHeader.Crc32;
					entry.LastWriteTime = fileHeader.LastWriteTime;
					entries.Add(entry);
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
				}
				else if (nextHeader == FileHeader.ALZ_FILE_HEADER_END_MAGIC)
				{
					break;
				}
			}
			return entries;
		}
	}
}
