using EggDotNet.Extensions;
using System;
using System.IO;

namespace EggDotNet.Format.Egg
{
	internal sealed class SolidHeader
	{
		public const int SOLID_HEADER_MAGIC = 0x24E5A060;

		public static SolidHeader Parse(Stream stream)
		{
			_ = stream.ReadByte();
			stream.ReadShort(out short _);
			Console.Error.WriteLine("SOLID compression not implemented.  May encounter errors.");

			return new SolidHeader();
		}
	}
}
