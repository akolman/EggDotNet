using System;
using System.Collections.Generic;
using System.IO;

#if NETSTANDARD2_0
using EggDotNet.Extensions;
using EggDotNet.InternalExtensions;
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Alz
{
    internal sealed class AlzEntry : EggFileEntryBase
	{
		private readonly int _id;
		private readonly FileHeader FileHeader;

		private AlzEntry(FileHeader header, int id)
		{
			_id = id;
			FileHeader = header;
		}

		public override int Id => _id;
		public override string Name => FileHeader.Name;
		public override long Position => FileHeader.StartPosition;

		public override long UncompressedSize => FileHeader.UncompressedSize;

		public override long CompressedSize => FileHeader.CompressedSize;

		public override uint Crc32 => FileHeader.Crc32;

		public override CompressionMethod CompressionMethod => FileHeader.CompressionMethod;

		public override bool IsEncrypted => false;

		public override long ExternalAttributes => 0;

#if NETSTANDARD2_1_OR_GREATER
#nullable enable
		public override DateTime? LastWriteTime => FileHeader.LastWriteTime;

		public override string? Comment => string.Empty; //TODO
#else
		public override DateTime LastWriteTime => FileHeader.LastWriteTime;

		public override string Comment => string.Empty; //TODO
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
					var entry = new AlzEntry(FileHeader.Parse(stream), entries.Count);
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
