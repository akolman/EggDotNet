﻿using EggDotNet.Extensions;
using System.Collections.Generic;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class AlzEntry
	{
		public int Id { get; private set; }
		public string? Name { get; private set; }
		public long Position { get; private set; }

		public long UncompressedSize { get; private set; }

		public long CompressedSize { get; private set; }

		public CompressionMethod CompressionMethod { get; private set; }

		public static List<AlzEntry> ParseEntries(Stream stream)
		{
			var entries = new List<AlzEntry>();

			while (true)
			{
				var entry = new AlzEntry();

				if (stream.ReadInt(out int nextHeader) && nextHeader == FileHeader.ALZ_FILE_HEADER_START_MAGIC)
				{
					var fileHeader = FileHeader.Parse(stream);
					entry.CompressedSize = fileHeader.CompressedSize;
					entry.UncompressedSize = fileHeader.UncompressedSize;
					entry.Name = fileHeader.Name;
					entry.Position = fileHeader.StartPosition;
					entries.Add(entry);
					break;
				}

				
			}

			return entries;
		}
	}
}
