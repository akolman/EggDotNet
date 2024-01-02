using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EggDotNet.Format.Egg
{
	/// <summary>
	/// Represents an egg archive header.  There will be one egg header per volume.
	/// </summary>
	internal sealed class Header
	{
		private static readonly short HEADER_SIZE_BYTES = 14;

		public static readonly int EGG_HEADER_MAGIC = 0x41474745;

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

			if (stream.Read(buffer) != HEADER_SIZE_BYTES)
			{
				throw new InvalidDataException("Failed reading EGG header");
			}

			var version = BitConverter.ToInt16(buffer.Slice(0, 2));
			var headerId = BitConverter.ToInt32(buffer.Slice(2, 4));
			var reserved = BitConverter.ToInt32(buffer.Slice(6, 4));
			var next = BitConverter.ToInt32(buffer.Slice(10, 4));
#else
			var buffer = new byte[HEADER_SIZE_BYTES];

			if (stream.Read(buffer, 0, buffer.Length) != HEADER_SIZE_BYTES)
			{
				throw new InvalidDataException("Failed reading EGG header");
			}

			var version = BitConverter.ToInt16(buffer.Take(2).ToArray(), 0);
			var headerId = BitConverter.ToInt32(buffer.Skip(2).Take(4).ToArray(), 0);
			var reserved = BitConverter.ToInt32(buffer.Skip(6).Take(4).ToArray(), 0);
			var next = BitConverter.ToInt32(buffer.Skip(10).Take(4).ToArray(), 0);
#endif
			SplitHeader splitHeader = null;
			SolidHeader solidHeader = null;
			if (next != EGG_HEADER_END_MAGIC)
			{
#if NETSTANDARD2_1_OR_GREATER
				Span<byte> extFieldBuffer = stackalloc byte[Global.HEADER_SIZE];
				var foundEnd = false;

				while (!foundEnd && stream.Read(extFieldBuffer) == Global.HEADER_SIZE)
				{
					var nextHeader = BitConverter.ToInt32(extFieldBuffer);
#else
				var extFieldBuffer = new byte[Global.HEADER_SIZE];
				var foundEnd = false;

				while (!foundEnd && stream.Read(extFieldBuffer, 0, extFieldBuffer.Length) == Global.HEADER_SIZE)
				{
					var nextHeader = BitConverter.ToInt32(extFieldBuffer, 0);
#endif
					switch (nextHeader)
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
