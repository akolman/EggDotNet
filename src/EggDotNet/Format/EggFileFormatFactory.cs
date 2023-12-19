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
		public static IEggFileFormat Create(Stream stream, Func<Stream, IEnumerable<Stream>> streamCallback, Func<string> pwCallback)
		{
			if (!stream.ReadInt(out int header))
			{
				throw new InvalidDataException("Could not read header from stream");
			}

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
