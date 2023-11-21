using EggDotNet.Exception;
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

		public SplitHeader? SplitHeader { get; private set; }

		public long HeaderEndPosition { get; private set; }

		private Header(short version, int headerId, int reserved, long headerEnd, SplitHeader? splitHeader)
		{
			Version = version;
			HeaderId = headerId;
			Reserved = reserved;
			SplitHeader = splitHeader;
			HeaderEndPosition = headerEnd;
		}

		public static Header Parse(Stream stream)
		{
			Debug.Assert(stream.Position == 4);

			if (!stream.ReadShort(out short version))
			{
				throw new BadDataException("Failed reading version from header");
			}

			if (!stream.ReadInt(out int headerId))
			{
				throw new BadDataException("Failed reading ID from header");
			}

			if (!stream.ReadInt(out int reserved))
			{
				throw new BadDataException("Failed reading from header");
			}

			SplitHeader? splitHeader = null;
			while (stream.ReadInt(out int nextHeaderOrEnd) && nextHeaderOrEnd != EGG_HEADER_END_MAGIC)
			{
				if (nextHeaderOrEnd == SplitHeader.SPLIT_HEADER_MAGIC)
				{
					splitHeader = SplitHeader.Parse(stream);
				}	
			}

			return new Header(version, headerId, reserved, stream.Position, splitHeader); //won't OF unless corrupt
		}
	}
}
