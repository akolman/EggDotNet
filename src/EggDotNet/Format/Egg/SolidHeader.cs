using EggDotNet.InternalExtensions;
using System;
using System.IO;

namespace EggDotNet.Format.Egg
{
    internal sealed class SolidHeader
	{
		public const int SOLID_HEADER_MAGIC = 0x24E5A060;

		public static SolidHeader Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buffer = stackalloc byte[3];
#else
			var buffer = new byte[3];
#endif
			if (stream.Read(buffer) != 3)
			{
				throw new InvalidDataException("Failed reading solid header");
			}

			Console.Error.WriteLine("SOLID compression not implemented.  May encounter errors.");

			return new SolidHeader();
		}
	}
}
