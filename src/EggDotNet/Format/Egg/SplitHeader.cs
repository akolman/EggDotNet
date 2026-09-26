using System.IO;

#if USE_SPAN
using System;
#else
using EggDotNet.InternalExtensions;
using BitConverter = EggDotNet.InternalExtensions.BitConverterWrapper;
#endif

namespace EggDotNet.Format.Egg
{
    internal sealed class SplitHeader
	{
		public const int SPLIT_HEADER_MAGIC = 0x24F5A262;

		public int PreviousFileId { get; private set; }

		public int NextFileId { get; private set; }

		private SplitHeader(int previousFileId, int nextFileId)
		{
			PreviousFileId = previousFileId;
			NextFileId = nextFileId;
		}

		public static SplitHeader Parse(Stream stream)
		{
#if USE_SPAN
			Span<byte> buffer = stackalloc byte[11];
#else
			var buffer = new byte[11];
#endif
			if (stream.Read(buffer) < 11)
			{
				throw new InvalidDataException("Failed reading split header");
			}

			var prevFileId = BitConverter.ToInt32(buffer.Slice(3, 4));
			var nextFileId = BitConverter.ToInt32(buffer.Slice(7, 4));

			return new SplitHeader(prevFileId, nextFileId);
		}
	}
}
