using EggDotNet.Extensions;
using System;
using System.IO;

namespace EggDotNet.Format.Egg
{
	internal sealed class WinFileInfo //: ExtraField2
	{
		public const int WIN_FILE_INFO_MAGIC_HEADER = 0x2C86950B;

		public DateTime LastModified { get; private set; }

		public WindowsFileAttributes WindowsFileAttributes { get; private set; }

		public static WinFileInfo Parse(Stream stream)
		{
			_ = stream.ReadByte();

			_ = stream.ReadShort(out short _);

			if (!stream.ReadLong(out long lastModTime))
			{

			}

			var attributes =  (WindowsFileAttributes)stream.ReadByte();

			return new WinFileInfo() { LastModified = Utilities.FromEggTime(lastModTime), WindowsFileAttributes = attributes };
		}
	}
}
