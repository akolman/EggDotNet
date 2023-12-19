using EggDotNet.Exceptions;
using EggDotNet.Extensions;
using System.Diagnostics;
using System.IO;

namespace EggDotNet.Format.Egg
{
	/// <summary>
	/// Represents an egg archive header.  There will be one egg header per volume.
	/// </summary>
	internal sealed class Header
	{
		public static readonly int EGG_HEADER_MAGIC = 0x41474745;

		public static readonly int EGG_HEADER_END_MAGIC = 0x08E28222;

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

			if (!stream.ReadShort(out short version))
			{
				throw new InvalidDataException("Failed reading version from header");
			}

			if (!stream.ReadInt(out int headerId))
			{
				throw new InvalidDataException("Failed reading ID from header");
			}

			if (!stream.ReadInt(out int reserved))
			{
				throw new InvalidDataException("Failed reading from header");
			}

			SplitHeader splitHeader = null;
			SolidHeader solidHeader = null;
			while (stream.ReadInt(out int nextHeaderOrEnd) && nextHeaderOrEnd != EGG_HEADER_END_MAGIC)
			{
				if (nextHeaderOrEnd == SplitHeader.SPLIT_HEADER_MAGIC)
				{
					splitHeader = SplitHeader.Parse(stream);
				}
				else if (nextHeaderOrEnd == SolidHeader.SOLID_HEADER_MAGIC)
				{
					solidHeader = SolidHeader.Parse(stream);
				}
			}

			return new Header(version, headerId, reserved, stream.Position, splitHeader, solidHeader); //won't OF unless corrupt
		}
	}
}
