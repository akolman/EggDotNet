using EggDotNet.Exception;
using EggDotNet.Extensions;
using EggDotNet.Format.Egg;
using System.Collections.Generic;
using System;
using System.IO;

namespace EggDotNet.Format
{
	internal static class EggFileFormatFactory
	{
		public static IEggFileFormat Create(Stream stream, Func<Stream, IEnumerable<Stream>>? streamCallback, Func<string>? pwCallback)
		{
			if (!stream.ReadInt(out int header))
			{
				throw new BadDataException("Could not read header from stream");
			}

			return header switch
			{
				var _ when header == Egg.Header.EGG_HEADER_MAGIC => new EggFormat(streamCallback, pwCallback),
				_ => throw new UnknownEggException()
			};
		}
	}
}
