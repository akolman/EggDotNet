using EggDotNet.Extensions;
using System;
using System.IO;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.Extensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Alz
{
	internal sealed class Header
	{
		public const int ALZ_HEADER_MAGIC = 0x015A4C41;

		public short Version { get; private set; }

		public short HeaderId { get; private set; }

		private Header(short version, short headerId)
		{
			Version = version;
			HeaderId = headerId;
		}

		public static Header Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> headerBuffer = stackalloc byte[4];
#else
			var headerBuffer = new byte[4];
#endif

			if (stream.Read(headerBuffer) != 4)
			{
				throw new InvalidDataException("Failed reading Alz header");
			}

			var version = BitConverter.ToInt16(headerBuffer.Slice(0, 2));
			var headerId = BitConverter.ToInt16(headerBuffer.Slice(2, 2));

			return new Header(version, headerId);
		}
	}
}
