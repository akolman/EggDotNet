using EggDotNet.InternalExtensions;
using System;
using System.IO;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
    internal sealed class FileHeader
	{
		public const int FILE_HEADER_MAGIC = 0x0A8590E3;

		public const int FILE_END_HEADER = 0x08E28222;

		public int FileId { get; private set; }

		public long FileLength { get; private set; }

		private FileHeader(int fileId, long fileLength)
		{
			FileId = fileId;
			FileLength = fileLength;
		}

		public static FileHeader Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> headerBuffer = stackalloc byte[12];
#else
			var headerBuffer = new byte[12];
#endif
			if (stream.Read(headerBuffer) != 12)
			{
				throw new InvalidDataException("Failed reading file entry header");
			}

			var fileId = BitConverter.ToInt32(headerBuffer.Slice(0, 4));
			var fileLength = BitConverter.ToInt64(headerBuffer.Slice(4, 8));

			return new FileHeader(fileId, fileLength);
		}
	}
}
