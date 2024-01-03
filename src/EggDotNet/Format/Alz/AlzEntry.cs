using System;
using System.Collections.Generic;
using System.IO;

#if NETSTANDARD2_0
using EggDotNet.Extensions;
using BitConverter = EggDotNet.Extensions.BitConverterWrapper;
#endif

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

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> nextHeaderBuf = stackalloc byte[Global.HEADER_SIZE];
#else
			var nextHeaderBuf = new byte[Global.HEADER_SIZE];
#endif

			while (stream.Read(nextHeaderBuf) == Global.HEADER_SIZE)
			{
				var nextHeader = BitConverter.ToInt32(nextHeaderBuf);

				if (nextHeader == FileHeader.ALZ_FILE_HEADER_START_MAGIC)
				{
					var entry = new AlzEntry
					{
						FileHeader = FileHeader.Parse(stream),
						Id = entries.Count
					};
					stream.Seek(entry.CompressedSize, SeekOrigin.Current);
					entries.Add(entry);
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
