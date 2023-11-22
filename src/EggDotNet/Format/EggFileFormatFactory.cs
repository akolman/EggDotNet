﻿using EggDotNet.Exception;
using EggDotNet.Extensions;
using EggDotNet.Format;
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
				throw new BadDataEggception("Could not read header from stream");
			}

			return header switch
			{
				var _ when header == Egg.Header.EGG_HEADER_MAGIC => new Egg.EggFormat(streamCallback, pwCallback),
				var _ when header == Alz.Header.ALZ_HEADER_MAGIC => new Alz.AlzFormat(),
				_ => throw new UnknownEggEggception()
			}; ; ;
		}
	}
}
