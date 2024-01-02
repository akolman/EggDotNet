using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System;
using System.IO;
using System.Linq;

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

			if (stream.Read(buffer) < 18)
			{
				throw new InvalidDataException("Failed reading block");
			}

			var compressionMethod = BitConverter.ToInt16(buffer.Slice(0, 2));
			var uncompSize = BitConverter.ToInt32(buffer.Slice(2, 4));
			var compSize = BitConverter.ToInt32((buffer.Slice(6, 4)));
			var crc = BitConverter.ToUInt32((buffer.Slice(10, 4)));
#else
			var buffer = new byte[18];

			if (stream.Read(buffer, 0, 18) < 18)
			{
				throw new InvalidDataException("Failed reading block");
			}

			var compressionMethod = BitConverter.ToInt16(buffer.Take(2).ToArray(), 0);
			var uncompSize = BitConverter.ToInt32(buffer.Skip(2).Take(4).ToArray(), 0);
			var compSize = BitConverter.ToInt32(buffer.Skip(6).Take(4).ToArray(), 0);
			var crc = BitConverter.ToUInt32(buffer.Skip(10).Take(4).ToArray(), 0);
#endif
			return new BlockHeader((CompressionMethod)(compressionMethod & 0xFF), compSize, uncompSize, stream.Position, crc);
		}
	}
}
