using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class WinFileInfo //: ExtraField2
	{
		public const int WIN_FILE_INFO_MAGIC_HEADER = 0x2C86950B;

		private const long MODDATE_EPOCH_TICKS = 504911232000000000;


		public DateTime LastModified { get; private set; }

		public static WinFileInfo Parse(Stream stream)
		{
			_ = stream.ReadByte();

			_ = stream.ReadShort(out short size);

			if (!stream.ReadLong(out long lastModTime))
			{

			}

			var modDate = new DateTime(lastModTime + MODDATE_EPOCH_TICKS);

			var attributes = stream.ReadByte();

			return new WinFileInfo() { LastModified = modDate };
		}
	}
}
