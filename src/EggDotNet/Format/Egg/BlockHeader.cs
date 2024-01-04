using EggDotNet.Exceptions;
using EggDotNet.InternalExtensions;
using System;
using System.IO;
using System.Linq;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

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
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buffer = stackalloc byte[18];
#else
			var buffer = new byte[18];
#endif
			if (stream.Read(buffer) < 18)
			{
				throw new InvalidDataException("Failed reading block");
			}

			var compressionMethod = BitConverter.ToInt16(buffer.Slice(0, 2));
			var uncompSize = BitConverter.ToInt32(buffer.Slice(2, 4));
			var compSize = BitConverter.ToInt32((buffer.Slice(6, 4)));
			var crc = BitConverter.ToUInt32((buffer.Slice(10, 4)));

			return new BlockHeader((CompressionMethod)(compressionMethod & 0xFF), compSize, uncompSize, stream.Position, crc);
		}
	}
}
