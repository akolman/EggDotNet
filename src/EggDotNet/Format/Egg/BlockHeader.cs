using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System;
using System.IO;

namespace EggDotNet.Format.Egg
{
	internal sealed class BlockHeader
	{
		public const int BLOCK_HEADER_MAGIC = 0x02B50C13;

		public const int BLOCK_HEADER_END_MAGIC = 0x08E28222;

		public CompressionMethod CompressionMethod { get; private set; }

		public int CompressedSize { get; private set; }

		public int UncompressedSize { get; private set; }

		public long BlockDataPosition { get; private set; }

		public uint Crc32 { get; private set; }

		private BlockHeader(CompressionMethod compressionMethod, int compressedSize, int uncompressedSize, long blockDataPosition, uint crc32)
		{
			CompressionMethod = compressionMethod;
			CompressedSize = compressedSize;
			UncompressedSize = uncompressedSize;
			BlockDataPosition = blockDataPosition;
			Crc32 = crc32;
		}

		public static BlockHeader Parse(Stream stream)
		{
			if (!stream.ReadShort(out short compressionMethod))
			{
				throw new InvalidDataException("Failed to read block compression method");
			}

			if (!stream.ReadInt(out int uncompSize))
			{
				throw new InvalidDataException("Failed to read block uncompressed size");
			}

			if (!stream.ReadInt(out int compSize))
			{
				throw new InvalidDataException("Failed to read block compressed size");
			}

			if (!stream.ReadUInt(out uint crc))
			{
				throw new InvalidDataException("Failed to read block checksum");
			}

			if (!stream.ReadInt(out int endHeader) && endHeader != BLOCK_HEADER_END_MAGIC)
			{
				Console.Error.WriteLine("Didn't find block end header");
			}

			return new BlockHeader((CompressionMethod)(compressionMethod & 0xFF), compSize, uncompSize, stream.Position, crc);
		}
	}
}
