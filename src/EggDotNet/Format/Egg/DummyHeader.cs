using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
using EggDotNet.InternalExtensions;
#endif

namespace EggDotNet.Format.Egg
{
	[ExcludeFromCodeCoverage]
	internal sealed class DummyHeader
	{
		public const int DUMMY_HEADER_MAGIC = 0x07463307;

		private readonly short _size;

		private DummyHeader(short size)
		{
			_size = size;
		}

		public static DummyHeader Parse(Stream stream)
		{
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> dummyHeader = stackalloc byte[3];
#else
			var dummyHeader = new byte[3];
#endif
			if (stream.Read(dummyHeader) != 3)
			{
				throw new InvalidDataException("Failed reading dummy header");
			}

			var dummySize = BitConverter.ToInt16(dummyHeader.Slice(1, 2));
			stream.Seek(dummySize, SeekOrigin.Current);
			return new DummyHeader(dummySize);
		}
	}
}
