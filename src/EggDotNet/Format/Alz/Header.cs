using EggDotNet.Extensions;
using System.IO;

namespace EggDotNet.Format.Alz
{
	internal sealed class Header
	{
		public static readonly int ALZ_HEADER_MAGIC = 0x015A4C41;

		public short Version { get; private set; }

		public short HeaderId { get; private set; }

		private Header(short version, short headerId)
		{
			Version = version;
			HeaderId = headerId;
		}

		public static Header Parse(Stream stream)
		{
			if (!stream.ReadShort(out short version))
			{
				throw new InvalidDataException("Could not read Alz version");
			}

			if (!stream.ReadShort(out short headerId))
			{
				throw new InvalidDataException("Could not read Alz header Id");
			}

			return new Header(version, headerId);
		}
	}
}
