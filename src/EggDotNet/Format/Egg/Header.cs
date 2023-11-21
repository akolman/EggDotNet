using EggDotNet.Exception;
using EggDotNet.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EggDotNet.Format.Egg
{
	internal class Header
	{
		public static readonly int EGG_HEADER_MAGIC = 0x41474745;

		public static readonly int EGG_HEADER_END_MAGIC = 0x08E28222;

		public short Version { get; private set; }

		public int HeaderId { get; private set; }

		public int Reserved { get; private set; }

		public SplitHeader? SplitHeader { get; private set; }

		public int HeaderEndPosition { get; private set; }

		private Header(short version, int headerId, int reserved, SplitHeader? splitHeader)
		{
			Version = version;
			HeaderId = headerId;
			Reserved = reserved;
			SplitHeader = splitHeader;
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
			EncryptHeader? encryptHeader = null;
			while (stream.ReadInt(out int nextHeaderOrEnd) && nextHeaderOrEnd != EGG_HEADER_END_MAGIC)
			{
				if (nextHeaderOrEnd == SplitHeader.SPLIT_HEADER_MAGIC)
				{
					splitHeader = SplitHeader.Parse(stream);
				}	
			}

			return new Header(version, headerId, reserved, splitHeader) { HeaderEndPosition = (int)stream.Position}; //won't OF unless corrupt
		}
	}
}
