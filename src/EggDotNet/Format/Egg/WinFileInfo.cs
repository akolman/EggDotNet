using EggDotNet.Extensions;
using System;
using System.IO;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.Extensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
	internal sealed class WinFileInfo //: ExtraField2
	{
		public const int WIN_FILE_INFO_MAGIC_HEADER = 0x2C86950B;

		public DateTime LastModified { get; private set; }

		public int WindowsFileAttributes { get; private set; }

		public static WinFileInfo Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> winFileBuffer = stackalloc byte[12];
#else
			var winFileBuffer = new byte[12];
#endif
			if (stream.Read(winFileBuffer) != 12)
			{
				throw new InvalidDataException("Failed reading windows file header");
			}

			var lastModTime = BitConverter.ToInt64(winFileBuffer.Slice(3, 8));
			var attributes = winFileBuffer[11];

			return new WinFileInfo() { LastModified = Utilities.FromEggTime(lastModTime), WindowsFileAttributes = attributes };
		}
	}
}
