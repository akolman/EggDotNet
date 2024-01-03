using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using EggDotNet.Format;
using System.Collections.Generic;
using System;
using System.IO;

namespace EggDotNet.Format
{
	internal static class EggFileFormatFactory
	{
		public static EggFileFormatBase Create(Stream stream, Func<Stream, IEnumerable<Stream>> streamCallback, Func<string> pwCallback)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> headerBuffer = stackalloc byte[Global.HEADER_SIZE];
			if (stream.Read(headerBuffer) != Global.HEADER_SIZE)
			{
				throw new InvalidDataException("Failed reading archive header");
			}

			var header = BitConverter.ToInt32(headerBuffer);
#else
			var headerBuffer = new byte[Global.HEADER_SIZE];
			if (stream.Read(headerBuffer, 0, Global.HEADER_SIZE) != Global.HEADER_SIZE)
			{
				throw new InvalidDataException("Failed reading archive header");
			}

			var header = BitConverter.ToInt32(headerBuffer, 0);
#endif
			if (header == Egg.Header.EGG_HEADER_MAGIC)
			{
				return new Egg.EggFormat(streamCallback, pwCallback);
			}
			else if (header == Alz.Header.ALZ_HEADER_MAGIC)
			{
				return new Alz.AlzFormat();
			}
			else
			{
				throw new UnknownEggException();
			}
		}
	}
}
