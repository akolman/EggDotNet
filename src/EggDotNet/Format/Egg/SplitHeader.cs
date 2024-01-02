using EggDotNet.Extensions;
using System;
using System.IO;
using System.Linq;

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
#if NETSTANDARD2_1_OR_GREATER
			Span<byte> buffer = stackalloc byte[11];
			if (stream.Read(buffer) < 11)
			{
				throw new InvalidDataException("Failed reading split header");
			}

			var prevFileId = BitConverter.ToInt32(buffer.Slice(3, 4));
			var nextFileId = BitConverter.ToInt32(buffer.Slice(7, 4));
#else
			var buffer = new byte[11];
			if (stream.Read(buffer, 0, 11) < 11)
			{
				throw new InvalidDataException("Failed reading split header");
			}

			var prevFileId = BitConverter.ToInt32(buffer.Skip(3).Take(4).ToArray(), 0);
			var nextFileId = BitConverter.ToInt32(buffer.Skip(7).Take(4).ToArray(), 0);
#endif

			return new SplitHeader(prevFileId, nextFileId);
		}
	}
}
