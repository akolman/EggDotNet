using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System;
using System.IO;
using System.Linq;

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
			if (stream.Read(headerBuffer) != 12)
			{
				throw new InvalidDataException("Failed reading file entry header");
			}

			var fileId = BitConverter.ToInt32(headerBuffer.Slice(0, 4));
			var fileLength = BitConverter.ToInt64(headerBuffer.Slice(4, 8));
#else
			var headerBuffer = new byte[12];
			if (stream.Read(headerBuffer, 0, 12) != 12)
			{
				throw new InvalidDataException("Failed reading file entry header");
			}

			var fileId = BitConverter.ToInt32(headerBuffer.Take(4).ToArray(), 0);
			var fileLength = BitConverter.ToInt64(headerBuffer.Skip(4).Take(8).ToArray(), 0);
#endif

			return new FileHeader(fileId, fileLength);
		}
	}
}
