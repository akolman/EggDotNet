using EggDotNet.Exceptions;
using System;
using System.IO;
using static EggDotNet.Callbacks;

#if NETSTANDARD2_0
using EggDotNet.InternalExtensions;
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format
{
	internal static class EggFileFormatFactory
	{
		public static EggFileFormatBase Create(Stream stream, SplitFileReceiverCallback streamCallback, FileDecryptPasswordCallback pwCallback)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> headerBuffer = stackalloc byte[Global.HEADER_SIZE];
#else
			var headerBuffer = new byte[Global.HEADER_SIZE];
#endif
			if (stream.Read(headerBuffer) != Global.HEADER_SIZE)
			{
				throw new InvalidDataException("Failed reading archive header");
			}

			var header = BitConverter.ToInt32(headerBuffer);

			switch (header)
			{
				case Egg.Header.EGG_HEADER_MAGIC:
					return new Egg.EggFormat(streamCallback, pwCallback);
				case Alz.Header.ALZ_HEADER_MAGIC:
					return new Alz.AlzFormat();
				default:
					throw new UnknownEggException();
			}
		}
	}
}
