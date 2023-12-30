using EggDotNet.Extensions;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzEntry
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public uint Crc32 { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		public EggArchiveEntry ToArchiveEntry(AlzFormat format, EggArchive archive)
		{
			return new EggArchiveEntry(format, archive)
			{
				FullName = Name,
				PositionInStream = Position,
				CompressedLength = CompressedSize,
				UncompressedLength = UncompressedSize,
				CompressionMethod = CompressionMethod,
				Comment = null,
				Archive = archive,
				Id = Id,
				Crc32 = Crc32
			};
		}

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
