using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System.IO;

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
			if (!stream.ReadInt(out int fileId))
			{
				throw new InvalidDataException("Failed reading ID from file header");
			}
			
			if (!stream.ReadLong(out long fileLength))
			{
				throw new InvalidDataException("Failed reading file length from file header");
			}

			return new FileHeader(fileId, fileLength);
		}
	}
}
