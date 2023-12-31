﻿using System;
using System.Diagnostics;
using System.IO;

#if NETSTANDARD2_0
using EggDotNet.Extensions;
using EggDotNet.InternalExtensions;
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
    /// <summary>
    /// Represents an egg archive header.  There will be one egg header per volume.
    /// </summary>
    internal sealed class Header
	{
		private const short HEADER_SIZE_BYTES = 14;

		public const int EGG_HEADER_MAGIC = 0x41474745;

		public const int EGG_HEADER_END_MAGIC = 0x08E28222;

		/// <summary>
		/// Gets the version associated with this <see cref="Header"/>.
		/// </summary>
		public short Version { get; private set; }

		public int HeaderId { get; private set; }

		public int Reserved { get; private set; }

		public SplitHeader SplitHeader { get; private set; }

		public SolidHeader SolidHeader { get; private set; }

		public long HeaderEndPosition { get; private set; }

		private Header(short version, int headerId, int reserved, long headerEnd, SplitHeader splitHeader, SolidHeader solidHeader)
		{
			Version = version;
			HeaderId = headerId;
			Reserved = reserved;
			SplitHeader = splitHeader;
			SolidHeader = solidHeader;
			HeaderEndPosition = headerEnd;
		}

		public static Header Parse(Stream stream)
		{
			Debug.Assert(stream.Position == 4);

#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buffer = stackalloc byte[HEADER_SIZE_BYTES];
#else
			var buffer = new byte[HEADER_SIZE_BYTES];
#endif
			if (stream.Read(buffer) != HEADER_SIZE_BYTES)
			{
				throw new InvalidDataException("Failed reading EGG header");
			}

			var version = BitConverter.ToInt16(buffer.Slice(0, 2));
			var headerId = BitConverter.ToInt32(buffer.Slice(2, 4));
			var reserved = BitConverter.ToInt32(buffer.Slice(6, 4));
			var next = BitConverter.ToInt32(buffer.Slice(10, 4));

			SplitHeader splitHeader = null;
			SolidHeader solidHeader = null;
			if (next != EGG_HEADER_END_MAGIC)
			{
#if NETSTANDARD2_1_OR_GREATER
				Span<byte> extFieldBuffer = stackalloc byte[Global.HEADER_SIZE];
#else
				var extFieldBuffer = new byte[Global.HEADER_SIZE];
#endif
				var foundEnd = false;

				while (!foundEnd && stream.Read(extFieldBuffer) == Global.HEADER_SIZE)
				{
					switch (BitConverter.ToInt32(extFieldBuffer))
					{
						case SplitHeader.SPLIT_HEADER_MAGIC:
							splitHeader = SplitHeader.Parse(stream);
							break;
						case SolidHeader.SOLID_HEADER_MAGIC:
							solidHeader = SolidHeader.Parse(stream);
							break;
						case EGG_HEADER_END_MAGIC:
							foundEnd = true;
							break;
						default:
							break;

					}
				}
			}

			return new Header(version, headerId, reserved, stream.Position, splitHeader, solidHeader);
		}
	}
}
