using EggDotNet.Extensions;
using System.IO;

namespace EggDotNet.Format.Egg
{
	internal sealed class SolidHeader
	{
		public const int SOLID_HEADER_MAGIC = 0x24E5A060;


		public static SolidHeader Parse(Stream stream)
		{
			var bitFlag = stream.ReadByte();

			stream.ReadShort(out short size);

			return new SolidHeader();
		}
	}
}
